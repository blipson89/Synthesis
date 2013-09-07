using System.Linq;
using Sitecore.Data;

namespace Synthesis.FieldTypes.Adapters
{
	public class PathAdapter : IPathAdapter
	{
		private readonly ItemPath _path;

		public PathAdapter(ItemPath path)
		{
			_path = path;
		}

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
		public string ContentPath
		{
			get { return _path.ContentPath; }
		}

		/// <summary>
		///     Gets the full path of the item.
		/// </summary>
		/// <value>
		///     The full path.
		/// </value>
		public string FullPath
		{
			get { return _path.FullPath; }
		}

		/// <summary>
		///     Gets a value indicating whether this item is a descendant of the /sitecore/content item.
		/// </summary>
		/// <value>
		///     <c>true</c> if this item is a descendant of the /sitecore/content item; otherwise, <c>false</c>.
		/// </value>
		public bool IsContentItem
		{
			get { return _path.IsContentItem; }
		}

		/// <summary>
		///     Gets a value indicating whether this item is a descendant of the /sitecore/media library item.
		/// </summary>
		/// <value>
		///     <c>true</c> if this item is a descendant of the /sitecore/media library; otherwise, <c>false</c>.
		/// </value>
		public bool IsMediaItem
		{
			get { return _path.IsMediaItem; }
		}

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
		public string MediaPath
		{
			get { return _path.MediaPath; }
		}

		/// <summary>
		///     Gets the path of the parent item.
		/// </summary>
		/// <value>
		///     The parent path.
		/// </value>
		public string ParentPath
		{
			get { return _path.ParentPath; }
		}

		/// <summary>
		///     Gets the path.
		/// </summary>
		/// <value>
		///     The path.
		/// </value>
		public string Path
		{
			get { return _path.Path; }
		}

		/// <summary>
		///     Determines whether an item is an ancestor of the current item.
		/// </summary>
		/// <param name="ancestorItem">Item to check.</param>
		/// <returns>
		///     <c>true</c> if  an item is an ancestor of the current item; otherwise, <c>false</c>.
		/// </returns>
		public bool IsAncestorOf(IStandardTemplateItem ancestorItem)
		{
			return _path.IsAncestorOf(ancestorItem.InnerItem);
		}

		/// <summary>
		///     Determines whether the current item is a descendant of another item.
		/// </summary>
		/// <param name="descendantItem">Item.</param>
		/// <returns>
		///     <c>true</c> if the current item is a descendant of the other item; otherwise, <c>false</c>.
		/// </returns>
		public bool IsDescendantOf(IStandardTemplateItem descendantItem)
		{
			return _path.IsDescendantOf(descendantItem.InnerItem);
		}
	}
}