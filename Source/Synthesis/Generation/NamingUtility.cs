using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Synthesis.Generation
{
	internal static class NamingExtensions
	{
		/// <summary>
		/// Converts a string into a valid .NET identifier
		/// </summary>
		public static string AsIdentifier(this string identifier)
		{
			// allow for fields that start with a number
			if (char.IsDigit(identifier[0]))
				identifier = "_" + identifier;

			return Regex.Replace(identifier, "[^a-zA-Z0-9_\\.]+", string.Empty);
		}

		/// <summary>
		/// Converts a string into a valid .NET identifer that is unique across a collection of existing values
		/// </summary>
		public static string AsNovelIdentifier(this string identifier, ICollection<string> existingValues)
		{
			return AsNovel(AsIdentifier(identifier), existingValues);
		}

		/// <summary>
		/// Novelizes a string, given a list of existing values, by appending a number to it until it becomes unique
		/// </summary>
		/// <returns>The novel name</returns>
		public static string AsNovel(this string identifier, ICollection<string> existingValues)
		{
			string fixedID = identifier;
			int index = 1;

			while (existingValues.Contains(fixedID))
			{
				fixedID = identifier + index;
				index++;
			}

			return fixedID;
		}
	}
}
