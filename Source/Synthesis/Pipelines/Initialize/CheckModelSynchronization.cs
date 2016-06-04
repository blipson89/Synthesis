using System;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.StringExtensions;

namespace Synthesis.Pipelines.Initialize
{
	public class CheckModelSynchronization
	{
		public string StartupCheckMode { get; set; }

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
			var syncStatus = SynthesisHelper.CheckSyncAll();

			foreach (var configuration in syncStatus)
			{
				var syncResult = configuration.Value;

				if (syncResult.AreTemplatesSynchronized) return;

				foreach (var template in syncResult)
				{
					if (!template.IsSynchronized)
						Log.Warn("Synthesis template desynchronization ({0}): {1}".FormatWith(configuration.Key.Name, template), this);
				}
			}
		}

		private void DoRegenerateSync()
		{
			var syncStatus = SynthesisHelper.CheckSyncAllAndRegenerate();

			foreach (var configuration in syncStatus)
			{
				var syncResult = configuration.Value;

				if (syncResult.AreTemplatesSynchronized) return;

				foreach (var template in syncResult)
				{
					if (!template.IsSynchronized)
						Log.Warn("Synthesis template desynchronization ({0}): {1}".FormatWith(configuration.Key.Name, template), this);
				}
			}
		}
	}
}
