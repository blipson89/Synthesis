namespace Synthesis.FieldTypes.Interfaces
{
	public interface IContentHubImageField : IImageField
	{
		/// <summary>
		/// Gets the StyleLabs Content ID
		/// </summary>
		string ContentId { get; }

		/// <summary>
		/// Gets the Thumbnail Src
		/// </summary>
		string ThumbnailSrc { get; }

		/// <summary>
		/// Gets the StyleLabs Content Type
		/// </summary>
		string ContentType { get; }

		/// <summary>
		/// Returns if the image is from Content Hub
		/// </summary>
		bool IsContentHub { get; }
	}
}