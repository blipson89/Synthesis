using System;
using System.Web;

namespace Synthesis.Utility
{
	/// <summary>
	/// Caches the result of checking if dynamic debug is enabled, which seems to not cache its result in the BCL.
	/// </summary>
	public static class DebugUtility
	{
		public static bool IsDynamicDebugEnabled { get { return DebugEnabled.Value; } }

		private static readonly Lazy<bool> DebugEnabled = new Lazy<bool>(() => HttpContext.Current.IsDebuggingEnabled);
	}
}
