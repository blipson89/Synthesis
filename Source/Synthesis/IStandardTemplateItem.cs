using System.Diagnostics.CodeAnalysis;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;

namespace Synthesis
{
	/// <summary>
	/// The interface implemented by all strongly typed items that is an analogue to the Standard Template in Sitecore
	/// </summary>
	public interface IStandardTemplateItem
	{
		/// <summary>
		/// Name of the item
		/// </summary>
		[IndexField("_name")]
		string Name { get; set; }

		/// <summary>
		/// Display name of the item (falls back to Name if Display Name is not present)
		/// </summary>
		[IndexField("__display_name")]
		string DisplayName { get; set; }

		/// <summary>
		/// The item's ID. If not yet created, returns ID.Null.
		/// </summary>
		[IndexField("_group")]
		ID Id { get; }

		/// <summary>
		/// The unique URI to the item, comprised of ID, language, database, and version.
		/// </summary>
		[IndexField("_uniqueid")]
		ItemUri Uri { get; }

		/// <summary>
		/// Item Template ID. If not yet created, returns ID.Null.
		/// </summary>
		[IndexField("_template")]
		ID TemplateId { get; }

		/// <summary>
		/// Item source database. If not yet created, returns null.
		/// </summary>
		Database Database { get; }

		/// <summary>
		/// Item version number
		/// </summary>
		[IndexField("_version")]
		int Version { get; }

		/// <summary>
		/// Gets if the item backing this instance is the latest version in its language
		/// </summary>
		[IndexField("_latestversion")]
		bool IsLatestVersion { get; }

		/// <summary>
		/// Item language. If not yet created, returns null.
		/// </summary>
		[IndexField("_language")]
		Language Language { get; }

		/// <summary>
		/// Source path data for the item
		/// </summary>
		ItemPath Paths { get; }

		/// <summary>
		/// Item statistics, i.e. created date
		/// </summary>
		ItemStatistics Statistics { get; }

		/// <summary>
		/// Provides access to the publishing framework
		/// </summary>
		ItemEditing Editing { get; }

		/// <summary>
		/// Gets a strongly typed version of the item's axes for relative querying
		/// </summary>
		StronglyTypedItemAxes Axes { get; }

		/// <summary>
		/// Gets the inner Sitecore Item. Avoid using this unless you absolutely have to.
		/// </summary>
		Item InnerItem { get; }

		/// <summary>
		/// Gets all base templates that this item's template inherits from
		/// </summary>
		[IndexField("_templatesimplemented")]
		ID[] TemplateIds { get; }

		/// <summary>
		/// Gets the URL to this item using the default LinkManager options. Returns null if not yet created.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Sitecore API convention")]
		string Url
		{
			get;
			set;
		}

		/// <summary>
		/// Adds a new item as a child of this item
		/// </summary>
		/// <typeparam name="TItem">The Synthesis type of the child to add. Must be a concrete template type.</typeparam>
		/// <param name="name">Name of the new item</param>
		/// <returns>The newly added child item</returns>
		TItem Add<TItem>(string name) where TItem : class, IStandardTemplateItem;

		/// <summary>
		/// Search field indexer. Provides raw access to index field values for search-backed instances.
		/// </summary>
		/// <param name="searchFieldName">The index field name to get the value of.</param>
		/// <returns>The field name, or null if it did not exist.</returns>
		string this[string searchFieldName] { get; }
	}
}
