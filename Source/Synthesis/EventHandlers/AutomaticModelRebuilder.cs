using System;
using System.Threading;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Synthesis.Utility;

namespace Synthesis.EventHandlers
{
	public class AutomaticModelRebuilder
	{
		protected static readonly Timer RegenerateTimer = new Timer(Regenerate);
		const int RegenerateDelayMs = 500;

		public void OnItemSavedMovedRenamedOrDeleted(object sender, EventArgs e)
		{
			var item = Event.ExtractParameter<Item>(e, 0);

			Process(item);
		}

		public virtual void Process(Item item)
		{
			// disable if compilation debug=true is in web.config (e.g. deployed off dev box)
			if (!DebugUtility.IsDynamicDebugEnabled) return;

			if (item == null) return;

			// not a template change, ignore
			if (item.TemplateID != TemplateIDs.Template && item.TemplateID != TemplateIDs.TemplateField) return;

			// not in master db, ignore
			if (!item.Database.Name.Equals("master")) return;

			// signal regeneration to start in the delay time
			// if additional events trigger this (like say saving 20 template fields)
			// they will push the timer further out and it will fire [delay] after the last event has completed.
			RegenerateTimer.Change(RegenerateDelayMs, Timeout.Infinite);
		}

		private static void Regenerate(object obj)
		{
			Log.Info("[AutomaticModelRebuilder] Regenerating Synthesis models due to change in template items.", typeof(AutomaticModelRebuilder));
			try
			{
				SynthesisHelper.RegenerateAll();
			}
			catch (Exception ex)
			{
				Log.Error("[AutomaticModelRebuilder] An error occurred rebuilding the model.", ex, typeof(AutomaticModelRebuilder));
			}
		}
	}
}
