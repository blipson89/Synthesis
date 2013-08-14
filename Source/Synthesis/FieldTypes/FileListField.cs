using System;
using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Synthesis.Utility;

namespace Synthesis.FieldTypes
{
	/// <summary>
	/// A field that is a reference to a media folder (a File Drop Area)
	/// </summary>
	public class FileListField : FieldType
	{
		private ReadOnlyCollection<Item> _mediaItems;

		public FileListField(Lazy<Field> field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the media items that are targeted by the file list field. If HasValue = false returns empty collection.
		/// </summary>
		public virtual ReadOnlyCollection<MediaItem> MediaItems
		{
			get { return ItemList.Cast<MediaItem>().ToList().AsReadOnly(); }
		}

		/// <summary>
		/// Gets the URLs to the items that are target by the file list field. If HasValue = false returns empty collection.
		/// </summary>
		public virtual ReadOnlyCollection<string> MediaUrls
		{
			get { return ItemList.Select(x => FieldUtility.GetMediaUrl(x)).ToList().AsReadOnly(); }
		}

		/// <summary>
		/// Determines if the file list field has any valid media items assigned to it
		/// </summary>
		public override bool HasValue
		{
			get 
			{
				if (InnerField == null) return false;

				var list = ItemList;
				return list != null && list.Count > 0; 
			}
		}

		protected ReadOnlyCollection<Item> ItemList
		{
			get
			{
				if (_mediaItems == null)
					_mediaItems = ((FileDropAreaField)InnerField).GetMediaItems().AsReadOnly();

				return _mediaItems;
			}
		}

		public FileDropAreaField ToFileDropAreaField()
		{
			return InnerField;
		}

		public static implicit operator FileDropAreaField(FileListField field)
		{
			return field.ToFileDropAreaField();
		}
	}
}
