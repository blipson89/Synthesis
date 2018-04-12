using System;
using Sitecore.Sites;

namespace Synthesis.Mvc.Utility
{
	public class DisplayModeSwitcher : IDisposable
	{
		private readonly DisplayMode _originalMode;

		public DisplayModeSwitcher(DisplayMode newDisplayMode)
		{
			_originalMode = Sitecore.Context.Site.DisplayMode;
			Sitecore.Context.Site.SetDisplayMode(newDisplayMode, DisplayModeDuration.Temporary);
		}

		public void Dispose()
		{
			Sitecore.Context.Site.SetDisplayMode(_originalMode, DisplayModeDuration.Temporary);
		}
	}
}