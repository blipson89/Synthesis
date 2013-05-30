using System.Collections.Generic;
using Sitecore.Data.Items;

namespace Synthesis.Templates
{
	/// <summary>
	/// Provides the list of templates that should have strongly typed items generated from them
	/// </summary>
	public interface ITemplateInputProvider
	{
		/// <summary>
		/// Gets all templates that should be used
		/// </summary>
		/// <returns></returns>
		IEnumerable<TemplateItem> CreateTemplateList();

		/// <summary>
		/// Checks if a template field should be included
		/// </summary>
		bool IsFieldIncluded(TemplateFieldItem field);

		/// <summary>
		/// Forces the provider to invalidate any internal caches and retrieve the latest data from Sitecore
		/// </summary>
		void Refresh();
	}
}
