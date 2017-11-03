using System;
using System.Web.Configuration;

namespace Synthesis.Utility
{
	/// <summary>
	/// Caches the result of checking if dynamic debug is enabled, which seems to not cache its result in the BCL.
	/// </summary>
	public static class DebugUtility
	{
		public static bool IsDynamicDebugEnabled => DebugEnabled.Value;

		private static readonly Lazy<bool> DebugEnabled = new Lazy<bool>(() =>
		{
			var config = WebConfigurationManager.GetSection("system.web/compilation") as CompilationSection;

			return config?.Debug ?? false;
		});
	}
}
