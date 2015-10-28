using System.Text.RegularExpressions;

namespace Synthesis.Utility
{
	public static class WildcardUtility
	{
		/// <summary>
		/// Checks if a string matches a wildcard argument (using regex)
		/// </summary>
		public static bool IsWildcardMatch(string input, string wildcards)
		{
			return Regex.IsMatch(input, "^" + Regex.Escape(wildcards).Replace("\\*", ".*").Replace("\\?", ".") + "$", RegexOptions.IgnoreCase);
		}
	}
}
