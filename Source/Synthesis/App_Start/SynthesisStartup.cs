using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.StringExtensions;
using Synthesis;
using Synthesis.Configuration;
using Synthesis.Generation;
using Synthesis.Synchronization;
using Synthesis.Utility;
using WebActivatorEx;
using Debug = System.Diagnostics.Debug;

[assembly: PostApplicationStartMethod(typeof(SynthesisStartup), "Initialize")]

namespace Synthesis
{
	public class SynthesisStartup
	{
		public static void Initialize()
		{
			InitializeDefaultConfiguration();
			InitializeSynchronization();
		}

		private static void InitializeDefaultConfiguration()
		{
			var config = Factory.GetConfigNode("synthesis/registerDefaultConfiguration");

			if (config == null || config.Attributes == null)
			{
				Log.Warn("sitecore/synthesis/registerDefaultConfiguration node was not defined; not registering a default configuration.", typeof(SynthesisStartup));
				return;
			}

			var mode = config.Attributes["value"];
			bool registerDefaultConfiguration;

			if (mode == null || !bool.TryParse(mode.Value, out registerDefaultConfiguration))
			{
				Log.Warn("The registerDefaultConfiguration setting was not specified or had a non-boolean value; not registering a default configuration.", typeof(SynthesisStartup));
				return;
			}

			if (!registerDefaultConfiguration) return;

			ProviderResolver.RegisterConfiguration(new ConfigurationProviderConfiguration());
		}

		/// <summary>
		/// Initializes application-start level automatic template sync checks
		/// </summary>
		private static void InitializeSynchronization()
		{
			// this will happen if the http handler is registered too early in the handlers list (before the sitecore handlers run)
			if (Globals.LinkDatabase == null) throw new InvalidOperationException("Link database was null. Most likely Sitecore has not yet been initialized for this request yet; you may need to change the HTTP module order so Synthesis initializes after Sitecore.");

			var config = Factory.GetConfigNode("synthesis/synchronizationSettings");

			if (config == null)
			{
				Log.Warn("sitecore/synthesis/synchronizationSettings node was not defined; not performing any synchronization checks.", typeof(SynthesisStartup));
				return;
			}

			var mode = config.SelectSingleNode("setting[@name='StartupCheckMode']");

			if (mode == null || mode.Attributes == null || mode.Attributes["value"] == null)
			{
				Log.Warn("The StartupCheckMode setting was not specified for model synchronization; not performing any synchronization checks.", typeof(SynthesisStartup));
				return;
			}

			switch (mode.Attributes["value"].InnerText)
			{
				case "Off": return;
				case "Log":
					DoLogSync();
					return;

				case "Regenerate":
					DoRegenerateSync(config);

					return;

				default:
					Log.Warn("Invalid setting \"" + mode.Attributes["value"].InnerText + "\" for StartupCheckMode. Assuming you mean Off and skipping synchronization.", typeof(SynthesisStartup));
					return;
			}
		}

		private static void DoLogSync()
		{
			foreach (var configuration in ProviderResolver.GetConfigurations())
			{
				var syncResult = GetSyncResult(configuration);

				if (!syncResult.AreTemplatesSynchronized)
				{
					foreach (var template in syncResult)
					{
						if (!template.IsSynchronized)
							Log.Warn("Synthesis template desynchronization ({0}): {1}".FormatWith(configuration.Name, template), typeof(SynthesisStartup));
					}
				}
			}
		}

		private static void DoRegenerateSync(XmlNode config)
		{
			foreach (var configuration in ProviderResolver.GetConfigurations())
			{
				var syncResult = GetSyncResult(configuration);

				if (syncResult.AreTemplatesSynchronized) return;

				if (!syncResult.AreTemplatesSynchronized)
				{
					foreach (var template in syncResult)
					{
						if (!template.IsSynchronized)
							Log.Warn("Synthesis template desynchronization ({0}): {1}".FormatWith(configuration.Name, template), typeof(SynthesisStartup));
					}
				}

				var projectPathNode = config.SelectSingleNode("setting[@name='StartupRegenerateProjectPath']");

				if (projectPathNode == null || projectPathNode.Attributes == null || projectPathNode.Attributes["value"] == null)
					throw new InvalidOperationException("The StartupRegenerateProjectPath setting must be specified when Synthesis Sync StartupCheckMode is set to Regenerate");

				var projectPathValue = projectPathNode.Attributes["value"].InnerText;

				if (projectPathValue == "auto")
				{
					projectPathValue = ResolveAutoProjectPath(configuration.GeneratorParametersProvider);

					if (projectPathValue == null)
						throw new InvalidOperationException("Unable to automatically find a valid project file to build. I looked at sibling and parent folders to the concrete file output path for *proj.");
				}
				else projectPathValue = ConfigurationUtility.ResolveConfigurationPath(projectPathValue);

				if (!File.Exists(projectPathValue))
					throw new InvalidOperationException("The auto-rebuild project file \"" + projectPathValue + "\" did not exist.");

				var metadata = configuration.CreateMetadataGenerator().GenerateMetadata();

				configuration.CreateCodeGenerator().Generate(metadata);

				var outputLogPath = Path.GetDirectoryName(projectPathValue) + Path.DirectorySeparatorChar + "synthesis-autobuild.log";

				if (!BuildUtility.BuildProject(projectPathValue, "Release", outputLogPath))
					Log.Error("Synthesis automatic project build on " + projectPathValue + " failed! Review the build log at " + outputLogPath + " to find out what happened.", typeof(SynthesisStartup));

				Log.Info("Synthesis detected templates were not synchronized for {0} and attempted to automatically rebuild {1} to correct the problem.".FormatWith(configuration.Name, projectPathValue), typeof(SynthesisStartup));
			}
		}

		private static string ResolveAutoProjectPath(IGeneratorParametersProvider parametersProvider)
		{
			var outputPath = parametersProvider.CreateParameters().ItemOutputPath;

			outputPath = Path.GetDirectoryName(outputPath);

			Debug.Assert(outputPath != null, "outputPath != null");

			return ResolveProject(new DirectoryInfo(outputPath));
		}

		private static string ResolveProject(DirectoryInfo rootDirectory)
		{
			var projectFiles = rootDirectory.GetFiles("*proj");
			if (projectFiles.Length > 0) return projectFiles[0].FullName;

			if (rootDirectory.Parent != null) return ResolveProject(rootDirectory.Parent);

			return null;
		}

		private static TemplateComparisonResultCollection GetSyncResult(IProviderConfiguration configuration)
		{
			var sw = new Stopwatch();

			sw.Start();
			TemplateComparisonResultCollection syncResult = configuration.CreateSyncEngine().AreTemplatesSynchronized();
			sw.Stop();

			Log.Info("Synthesis Template synchronization check for {0} completed in {1} ms".FormatWith(configuration.Name, sw.ElapsedMilliseconds), typeof(SynthesisStartup));

			return syncResult;
		}
	}
}
