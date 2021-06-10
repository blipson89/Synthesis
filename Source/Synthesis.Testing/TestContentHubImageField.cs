using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Testing
{
	public class TestContentHubImageField : TestImageField, IContentHubImageField
	{
		public TestContentHubImageField(string url, string contentId = null, string thumbnailSrc = null, string contentType = null, bool isContentHub = true,
			int? width = null, int? height = null, string alternateText = null)
			: base(url, width, height, alternateText)
		{
			ContentId = contentId;
			ThumbnailSrc = thumbnailSrc;
			ContentType = contentType;
			IsContentHub = isContentHub;
		}

		/// <summary>
		///     Gets the content ID of the image
		/// </summary>
		public string ContentId { get; set; }

		/// <summary>
		///     Gets the thumbnail src of the image
		/// </summary>
		public string ThumbnailSrc { get; set; }

		/// <summary>
		///     Gets the content type of the image
		/// </summary>
		public string ContentType { get; set; }

		/// <summary>
		///     Determine if image is from Content Hub or Media Library
		/// </summary>
		public bool IsContentHub { get; set; }

		/// <summary>
		///     Renders the field using a Sitecore FieldRenderer and returns the result
		/// </summary>
		public new string RenderedValue
		{
			get
			{
				if (IsContentHub)
				{
					string tag = $"<img stylelabs-content-type=\"{ContentType ?? string.Empty}\" mediaid=\"\" src=\"{Url ?? string.Empty}\" height=\"{Height}\" ";

					tag += $" alt=\"{AlternateText ?? string.Empty}\" stylelabs-content-id=\"{ContentId ?? string.Empty}\" width=\"{Width}\" thumbnailsrc=\"{ThumbnailSrc ?? string.Empty}\" format=\"\" />";

					return tag;
				}
				else
				{
					string tag = $"<img src=\"{Url ?? string.Empty}\" alt=\"{AlternateText ?? string.Empty}\"";
					if (Width.HasValue)
						tag += " width=\"" + Width.Value + "\"";

					if (Height.HasValue)
						tag += " height=\"" + Height.Value + "\"";

					return tag + " />";
				}

			}
		}
	}
}