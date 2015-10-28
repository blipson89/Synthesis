using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Converters;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Synthesis.FieldTypes.Adapters;

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
		/// The searchable content of the item (e.g. all text field values in the index).
		/// It is an error to read this field or use it outside of a search filter context.
		/// </summary>
		[IndexField("_content")]
		string SearchableContent { get; }

		/// <summary>
		/// The item's ID. If not yet created, returns ID.Null.
		/// </summary>
		[IndexField("_group")]
		[TypeConverter(typeof(IndexFieldIDValueConverter))]
		ID Id { get; }

		/// <summary>
		/// The item's parent ID
		/// </summary>
		[IndexField("_parent")]
		[TypeConverter(typeof(IndexFieldIDValueConverter))]
		ID ParentId { get; }

		/// <summary>
		/// The unique URI to the item, comprised of ID, language, database, and version.
		/// </summary>
		[IndexField("_uniqueid")]
		ItemUri ItemUri { get; }

		/// <summary>
		/// Item Template ID.
		/// </summary>
		[IndexField("_template")]
		[TypeConverter(typeof(IndexFieldIDValueConverter))]
		ID TemplateId { get; }

		/// <summary>
		/// Gets all base templates that this item's template inherits from
		/// </summary>
		[IndexField("_templatesimplemented")]
		[TypeConverter(typeof(IndexFieldEnumerableConverter))]
		ID[] TemplateIds { get; }

		/// <summary>
		/// Gets an enumerable of the IDs of all ancestors. Generally used to filter by path when using search.
		/// </summary>
		[IndexField("_path")]
		[TypeConverter(typeof(IndexFieldEnumerableConverter))]
		IEnumerable<ID> AncestorIds { get; }

		/// <summary>
		/// Gets the full path of the item
		/// </summary>
		[IndexField("_fullpath")]
		string Path { get; }

		/// <summary>
		/// Item source database.
		/// </summary>
		IDatabaseAdapter Database { get; }

		/// <summary>
		/// The name of the item source database
		/// </summary>
		[IndexField("_database")]
		string DatabaseName { get; }

		/// <summary>
		/// Item version number
		/// </summary>
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
		/// When this version was created
		/// </summary>
		[IndexField("__smallcreateddate")]
		DateTime CreatedDate { get; }

		/// <summary>
		/// Who created this version
		/// </summary>
		[IndexField("parsedcreatedby")]
		string CreatedBy { get; }

		/// <summary>
		/// When this version was last updated
		/// </summary>
		[IndexField("__smallupdateddate")]
		DateTime Updated { get; }

		/// <summary>
		/// Who last updated this version
		/// </summary>
		[IndexField("parsedupdatedby")]
		string UpdatedBy { get; }

		/// <summary>
		/// Provides access to the publishing framework
		/// </summary>
		IEditingAdapter Editing { get; }

		/// <summary>
		/// Gets a strongly typed version of the item's axes for relative querying
		/// </summary>
		IAxesAdapter Axes { get; }

		/// <summary>
		/// Gets the inner Sitecore Item. Avoid using this unless you absolutely have to.
		/// </summary>
		Item InnerItem { get; }

		/// <summary>
		/// Gets the URL to this item using the default LinkManager options. Returns null if not yet created.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Sitecore API convention")]
		string Url { get; set; }

		/// <summary>
		/// Adds a new item as a child of this item
		/// </summary>
		/// <typeparam name="TItem">The Synthesis type of the child to add. Must be a concrete template type.</typeparam>
		/// <param name="name">Name of the new item</param>
		/// <returns>The newly added child item</returns>
		TItem Add<TItem>(string name) where TItem : class, IStandardTemplateItem;

		/// <summary>
		/// Gets children of the item. This is an alias to Axes.GetChildren() so it's in a familiar location.
		/// </summary>
		IEnumerable<IStandardTemplateItem> Children { get; }

		/// <summary>
		/// Search field indexer. Provides raw access to index field values for search-backed instances.
		/// </summary>
		/// <param name="searchFieldName">The index field name to get the value of.</param>
		/// <returns>The field name, or null if it did not exist.</returns>
		string this[string searchFieldName] { get; }
	}
}