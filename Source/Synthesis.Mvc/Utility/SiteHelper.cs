using System.Collections.Generic;
using Sitecore;

namespace Synthesis.Mvc.Utility
{
	class SiteHelper
	{
		private static readonly HashSet<string> InvalidSites = new HashSet<string> { "shell", "core", "login", "admin", "service", "modules_shell", "scheduler", "publisher", "system" };
		public static bool IsValidSite()
		{
			return !InvalidSites.Contains(Context.Site.Name);
		}
	}
}
