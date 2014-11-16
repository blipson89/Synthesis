using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Globalization;

namespace Synthesis.FieldTypes.Adapters
{
	public interface IDatabaseAdapter
	{
		/// <summary>
		/// Gets the name of the database.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// The name.
		/// </value>
		string Name { get; }

		Database InnerDatabase { get; }

		/// <summary>
		/// Gets an item from a path.
		/// 
		/// </summary>
		/// <param name="itemId">The item id.</param>
		/// <returns/>
		IStandardTemplateItem GetItem(ID itemId);

		/// <summary>
		/// Gets an item from a path.
		/// 
		/// </summary>
		/// <param name="itemId">The item id.</param><param name="language">The language.</param>
		/// <returns/>
		IStandardTemplateItem GetItem(ID itemId, Language language);

		/// <summary>
		/// Gets an item from a path.
		/// 
		/// </summary>
		/// <param name="itemId">The item id.</param><param name="language">The language.</param><param name="version">The version.</param>
		/// <returns/>
		IStandardTemplateItem GetItem(ID itemId, Language language, Version version);

		/// <summary>
		/// Gets an item from a path.
		/// 
		/// </summary>
		/// <param name="path">The path.</param>
		/// <returns/>
		IStandardTemplateItem GetItem(string path);

		/// <summary>
		/// Gets an item from a path.
		/// 
		/// </summary>
		/// <param name="path">The path.</param><param name="language">The language.</param>
		/// <returns>
		/// The item.
		/// </returns>
		IStandardTemplateItem GetItem(string path, Language language);

		/// <summary>
		/// Gets an item from a path.
		/// 
		/// </summary>
		/// <param name="path">The path.</param><param name="language">The language.</param><param name="version">The version.</param>
		/// <returns>
		/// The item.
		/// </returns>
		IStandardTemplateItem GetItem(string path, Language language, Version version);

		/// <summary>
		/// Gets the item.
		/// 
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <returns/>
		IStandardTemplateItem GetItem(DataUri uri);

		/// <summary>
		/// Gets the languages defined in the database.
		/// 
		/// </summary>
		/// 
		/// <returns/>
		LanguageCollection GetLanguages();

		/// <summary>
		/// Gets the root item of the database.
		/// 
		/// </summary>
		/// 
		/// <returns/>
		IStandardTemplateItem GetRootItem();

		/// <summary>
		/// Gets the root item of a database.
		/// 
		/// </summary>
		/// <param name="language">The language.</param>
		/// <returns/>
		IStandardTemplateItem GetRootItem(Language language);


		/// <summary>
		/// Performs a Sitecore Xpath query in the database.
		/// 
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns/>
		IStandardTemplateItem[] SelectItems(string query);

		/// <summary>
		/// Performs a Sitecore Xpath query in the database.
		/// 
		/// </summary>
		/// <param name="query">The query.</param>
		/// <returns/>
		IStandardTemplateItem SelectSingleItem(string query);


	}
}
