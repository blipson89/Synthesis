namespace Synthesis.FieldTypes.Adapters
{
	public interface IPathAdapter
	{
		/// <summary>
		///     Gets the content path.
		/// </summary>
		/// <value>
		///     The content path.
		/// </value>
		/// <remarks>
		///     If the item is a descendant of /sitecore/content the path relative to
		///     /sitecore/content is returned with a slash '/' prepended. If not, the absolute path is returned.
		/// </remarks>
		/// <example>
		///     Item item = Sitecore.Context.Database.Items["/sitecore/content/Home/News"];
		///     string s = item.Paths.Path; // /Home/News
		/// </example>
		string ContentPath { get; }

		/// <summary>
		///     Gets the full path of the item.
		/// </summary>
		/// <value>
		///     The full path.
		/// </value>
		string FullPath { get; }

		/// <summary>
		///     Gets a value indicating whether this item is a descendant of the /sitecore/content item.
		/// </summary>
		/// <value>
		///     <c>true</c> if this item is a descendant of the /sitecore/content item; otherwise, <c>false</c>.
		/// </value>
		bool IsContentItem { get; }

		/// <summary>
		///     Gets a value indicating whether this item is a descendant of the /sitecore/media library item.
		/// </summary>
		/// <value>
		///     <c>true</c> if this item is a descendant of the /sitecore/media library; otherwise, <c>false</c>.
		/// </value>
		bool IsMediaItem { get; }

		/// <summary>
		///     Gets the media path.
		/// </summary>
		/// <value>
		///     The media path.
		/// </value>
		/// <remarks>
		///     If the item is a descendant of /sitecore/media library the path relative to
		///     /sitecore/media library is returned with a slash '/' prepended. If not, the absolute path is returned.
		/// </remarks>
		/// <example>
		///     Item item = Sitecore.Context.Database.Items["/sitecore/media library/Buildings/House"];
		///     string s = item.Paths.Path; // /Buildings/House
		/// </example>
		string MediaPath { get; }

		/// <summary>
		///     Gets the path of the parent item.
		/// </summary>
		/// <value>
		///     The parent path.
		/// </value>
		string ParentPath { get; }

		/// <summary>
		///     Gets the path.
		/// </summary>
		/// <value>
		///     The path.
		/// </value>
		string Path { get; }

		/// <summary>
		///     Determines whether an item is an ancestor of the current item.
		/// </summary>
		/// <param name="ancestorItem">Item to check.</param>
		/// <returns>
		///     <c>true</c> if  an item is an ancestor of the current item; otherwise, <c>false</c>.
		/// </returns>
		bool IsAncestorOf(IStandardTemplateItem ancestorItem);

		/// <summary>
		///     Determines whether the current item is a descendant of another item.
		/// </summary>
		/// <param name="descendantItem">Item.</param>
		/// <returns>
		///     <c>true</c> if the current item is a descendant of the other item; otherwise, <c>false</c>.
		/// </returns>
		bool IsDescendantOf(IStandardTemplateItem descendantItem);
	}
}