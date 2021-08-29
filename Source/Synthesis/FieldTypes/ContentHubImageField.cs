using Sitecore.Data;
using Sitecore.Data.Items;
using Synthesis.FieldTypes.Interfaces;
using System;

namespace Synthesis.FieldTypes
{
	public class ContentHubImageField : ImageField, IContentHubImageField
	{
		private string _contentId;
		private string _src;
		private string _thumbnailSrc;
		private string _contentType;

		public ContentHubImageField(LazyField field, string indexValue) : base(field, indexValue)
		{
		}

		public string ContentId
		{
			get
			{
				if (!IsContentHub)
				{
					return string.Empty;
				}

				if (_contentId != null)
				{
					return _contentId;
				}

				return _contentId = GetAttribute("stylelabs-content-id");
			}
		}

		public string ThumbnailSrc
		{
			get
			{
				if (!IsContentHub)
				{
					return string.Empty;
				}

				if (_thumbnailSrc != null)
				{
					return _thumbnailSrc;
				}

				return _thumbnailSrc = GetAttribute("thumbnailsrc");
			}
		}

		public string ContentType
		{
			get
			{
				if (!IsContentHub)
				{
					return string.Empty;
				}

				if (_contentType != null)
				{
					return _contentType;
				}

				return _contentType = GetAttribute("stylelabs-content-type");
			}
		}

		public override string Url
		{
			get
			{
				if (!IsContentHub)
				{
					return base.Url;
				}

				if (_src != null)
				{
					return _src;
				}

				return _src = GetAttribute("src");
			}
		}

		public override MediaItem MediaItem
		{
			get
			{
				if (!IsContentHub)
				{
					return base.MediaItem;
				}

				return null;
			}
		}

		public override ID MediaItemId
		{
			get
			{
				if (!IsContentHub)
				{
					return base.MediaItemId;
				}

				return ID.Null;
			}
			set
			{
				if (!IsContentHub)
				{
					base.MediaItemId = value;
				}

				throw new InvalidOperationException("Media Item ID cannot be set on a ContentHub image");
			}
		}

		public override bool HasValue
		{
			get
			{
				if (!IsContentHub)
				{
					return base.HasValue;
				}

				return !string.IsNullOrEmpty(GetAttribute("src"));
			}
		}


		public bool IsContentHub
		{
			get
			{
				if (InnerField == null) return false;
				return InnerField.Value?.Contains("stylelabs") ?? false;
			}
		}
	}
}
