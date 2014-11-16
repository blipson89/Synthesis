using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Xml;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Synthesis.Configuration;
using Synthesis.Synchronization;
using Synthesis.Utility;
using Debug = System.Diagnostics.Debug;

namespace Synthesis
{
	/// <summary>
	/// This HTTPModule is capable of automatically regenerating or logging if the generated model is not matching what's currently in Sitecore
	/// The sync check is very fast, less than a second on most normal sized Sitecore installations.
	/// </summary>
	public class AutoSyncModule : IHttpModule
	{
		public void Dispose()
		{
			// do nothing
		}

		private static readonly object SyncRoot = new object();
		private static bool _initialized;

		/// <summary>
		/// Resets the module instance to be uninitialized. Useful to force a reinitialization for testing.
		/// </summary>
		public static void Reset()
		{
			_initialized = false;
		}

		public void Init(HttpApplication context)
		{
			// "application_start" code fires once
			if (!_initialized)
			{
				lock (SyncRoot)
				{
					if (!_initialized)
					{
						InitializeSynchronization();
						_initialized = true;
					}
				}
			}
		}

		/// <summary>
		/// Initializes application-start level automatic template sync checks
		/// </summary>
		private void InitializeSynchronization()
		{
			// this will happen if the http handler is registered too early in the handlers list (before the sitecore handlers run)
			if (Globals.LinkDatabase == null) throw new InvalidOperationException("Link database was null. Most likely Sitecore has not yet been initialized for this request yet; you may need to change the HTTP module order so Synthesis initializes after Sitecore.");

			var config = Factory.GetConfigNode("synthesis/synchronizationSettings");

			if (config == null)
			{
				Log.Warn("sitecore/synthesis/synchronizationSettings node was not defined; not performing any synchronization checks.", this);
				return;
			}

			var mode = config.SelectSingleNode("setting[@name='StartupCheckMode']");

			if (mode == null || mode.Attributes == null || mode.Attributes["value"] == null)
			{
				Log.Warn("The StartupCheckMode setting was not specified for model synchronization; not performing any synchronization checks.", this);
				return;
			}

			switch(mode.Attributes["value"].InnerText)
			{
				case "Off": return;
				case "Log":
					DoLogSync();

					return;

				case "Regenerate":
					DoRegenerateSync(config);

					return;

				default:
					Log.Warn("Invalid setting \"" + mode.Attributes["value"].InnerText + "\" for StartupCheckMode. Assuming you mean Off and skipping synchronization.", this);
					return;
			}
		}

		private void DoRegenerateSync(XmlNode config)
		{
			var syncResult = GetSyncResult();

			if (syncResult.AreTemplatesSynchronized) return;

			if (!syncResult.AreTemplatesSynchronized)
			{
				foreach (var template in syncResult)
				{
					if (!template.IsSynchronized)
						Log.Warn("Synthesis template desynchronization: " + template, this);
				}
			}

			var projectPathNode = config.SelectSingleNode("setting[@name='StartupRegenerateProjectPath']");

			if (projectPathNode == null || projectPathNode.Attributes == null || projectPathNode.Attributes["value"] == null) 
				throw new InvalidOperationException("The StartupRegenerateProjectPath setting must be specified when Synthesis Sync StartupCheckMode is set to Regenerate");

			var projectPathValue = projectPathNode.Attributes["value"].InnerText;

			if (projectPathValue == "auto")
			{
				projectPathValue = ResolveAutoProjectPath();

				if (projectPathValue == null)
					throw new InvalidOperationException("Unable to automatically find a valid project file to build. I looked at sibling and parent folders to the concrete file output path for *proj.");
			}
			else projectPathValue = ConfigurationUtility.ResolveConfigurationPath(projectPathValue);

			if (!File.Exists(projectPathValue)) 
				throw new InvalidOperationException("The auto-rebuild project file \"" + projectPathValue + "\" did not exist.");

			ProviderResolver.CreateGenerator().GenerateToDisk();

			var outputLogPath = Path.GetDirectoryName(projectPathValue) + Path.DirectorySeparatorChar + "synthesis-autobuild.log";

			if (!BuildUtility.BuildProject(projectPathValue, "Release", outputLogPath))
				Log.Error("Synthesis automatic project build on " + projectPathValue + " failed! Review the build log at " + outputLogPath + " to find out what happened.", this);

			Log.Info("Synthesis detected templates were not synchronized and attempted to automatically rebuild " + projectPathValue + " to correct the problem.", this);
		}

		private void DoLogSync()
		{
			var syncResult = GetSyncResult();

			if (!syncResult.AreTemplatesSynchronized)
			{
				foreach (var template in syncResult)
				{
					if (!template.IsSynchronized)
						Log.Warn("Synthesis template desynchronization: " + template, this);
				}
			}
		}
		
		private string ResolveAutoProjectPath()
		{
			var outputPath = ProviderResolver.Current.GeneratorParametersProvider.CreateParameters().ItemOutputPath;

			outputPath = Path.GetDirectoryName(outputPath);

			Debug.Assert(outputPath != null, "outputPath != null");

			return ResolveProject(new DirectoryInfo(outputPath));
		}

		private string ResolveProject(DirectoryInfo rootDirectory)
		{
			var projectFiles = rootDirectory.GetFiles("*proj");
			if (projectFiles.Length > 0) return projectFiles[0].FullName;

			if (rootDirectory.Parent != null) return ResolveProject(rootDirectory.Parent);

			return null;
		}

		private TemplateComparisonResultCollection GetSyncResult()
		{
			var sw = new Stopwatch();

			sw.Start();
			TemplateComparisonResultCollection syncResult = ProviderResolver.CreateSyncEngine().AreTemplatesSynchronized();
			sw.Stop();

			Log.Info("Synthesis Template synchronization check completed in " + sw.ElapsedMilliseconds + " ms", this);

			return syncResult;
		}
	}
}
