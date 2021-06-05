using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using Synthesis.FieldTypes.Interfaces;
using Synthesis.Utility;
using System;

namespace Synthesis.FieldTypes
{
	public class FileField : XmlFieldType, IFileField
	{
		private string _url;
		private MediaItem _mediaItem;

		public FileField(LazyField field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the URL to the media item. If HasValue is false returns an empty string.
		/// </summary>
		public virtual string Url
		{
			get
			{
				if (_url != null) return _url;

				Item mediaItem = MediaItem;
				if (mediaItem != null) return _url = FieldUtility.GetMediaUrl(mediaItem);

				return _url = string.Empty;
			}
		}

		/// <summary>
		/// Gets the ID of the media item the mediafield references
		/// </summary>
		public virtual ID MediaItemId
		{
			get
			{
				return ((Sitecore.Data.Fields.FileField)InnerField).MediaID;
			}
			set
			{
				SetFieldValue(delegate
					{
					var field = (Sitecore.Data.Fields.FileField)InnerField;
					field.MediaID = value;
					field.Src = MediaManager.GetMediaUrl(InnerItem); // setting this per content api cookbook - odd that it'd be required though
				});
			}
		}

		/// <summary>
		/// The MediaItem that is the target of the file field
		/// </summary>
		public virtual MediaItem MediaItem
		{
			get
			{
				if (_mediaItem == null)
				{
					var fileField = (Sitecore.Data.Fields.FileField)InnerField;

					_mediaItem = fileField.MediaDatabase.GetItem(MediaItemId);
				}


				return _mediaItem;
			}
		}

		/// <summary>
		/// Gets a stream of the binary data in the file field. Make sure to dispose of the object when done!
		/// </summary>
		public MediaStream MediaStream
		{
			get
			{
				if (!HasValue) throw new InvalidOperationException("Media item had no value, cannot get the stream.");

				var item = MediaItem;

				if (item == null) return null;

				return MediaManager.GetMedia(item).GetStream();
			}
		}

		/// <summary>
		/// Checks if the media field has a reference value. Does not validate that the referenced ID actually exists.
		/// </summary>
		public override bool HasValue
		{
			get
			{
				if (InnerField == null)
				{
					return false;
				}

				return !(MediaItemId == (ID) null || MediaItemId.IsNull);
			}
		}
	}
}
