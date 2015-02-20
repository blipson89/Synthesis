using System;
using System.Diagnostics;
using System.IO;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.StringExtensions;
using Synthesis.Configuration;
using Synthesis.Generation;
using Synthesis.Synchronization;
using Synthesis.Utility;
using Debug = System.Diagnostics.Debug;

namespace Synthesis.Pipelines.Initialize
{
	public class CheckModelSynchronization
	{
		public string StartupCheckMode { get; set; }
		public string StartupRegenerateProjectPath { get; set; }

		public void Process(PipelineArgs args)
		{
			// this will happen if the http handler is registered too early in the handlers list (before the sitecore handlers run)
			if (Globals.LinkDatabase == null) throw new InvalidOperationException("Link database was null. Most likely Sitecore has not yet been initialized for this request yet; you may need to change the HTTP module order so Synthesis initializes after Sitecore.");

			switch (StartupCheckMode)
			{
				case "Off": return;
				case "Log":
					DoLogSync();
					return;

				case "Regenerate":
					DoRegenerateSync();

					return;

				default:
					Log.Warn("Invalid setting \"" + StartupCheckMode + "\" for StartupCheckMode. Assuming you mean Off and skipping synchronization.", this);
					return;
			}
		}

		private void DoLogSync()
		{
			foreach (var configuration in ProviderResolver.GetConfigurations())
			{
				var syncResult = GetSyncResult(configuration);

				if (!syncResult.AreTemplatesSynchronized)
				{
					foreach (var template in syncResult)
					{
						if (!template.IsSynchronized)
							Log.Warn("Synthesis template desynchronization ({0}): {1}".FormatWith(configuration.Name, template), this);
					}
				}
			}
		}

		private void DoRegenerateSync()
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
							Log.Warn("Synthesis template desynchronization ({0}): {1}".FormatWith(configuration.Name, template), this);
					}
				}

				string projectPathValue = StartupRegenerateProjectPath;

				if (StartupRegenerateProjectPath == "auto" || string.IsNullOrWhiteSpace(StartupRegenerateProjectPath))
				{
					projectPathValue = ResolveAutoProjectPath(configuration.GeneratorParametersProvider.CreateParameters(configuration.Name));

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
					Log.Error("Synthesis automatic project build on " + projectPathValue + " failed! Review the build log at " + outputLogPath + " to find out what happened.", this);

				Log.Info("Synthesis detected templates were not synchronized for {0} and attempted to automatically rebuild {1} to correct the problem.".FormatWith(configuration.Name, projectPathValue), this);
			}
		}

		private string ResolveAutoProjectPath(GeneratorParameters parameters)
		{
			var outputPath = parameters.ItemOutputPath;

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

		private TemplateComparisonResultCollection GetSyncResult(IProviderConfiguration configuration)
		{
			var sw = new Stopwatch();

			sw.Start();
			TemplateComparisonResultCollection syncResult = configuration.CreateSyncEngine().AreTemplatesSynchronized();
			sw.Stop();

			Log.Info("Synthesis Template synchronization check for {0} completed in {1} ms".FormatWith(configuration.Name, sw.ElapsedMilliseconds), this);

			return syncResult;
		}
	}
}
