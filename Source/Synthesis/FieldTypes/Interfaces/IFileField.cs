using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace Synthesis.FieldTypes.Interfaces
{
	public interface IFileField : IFieldType
	{
		/// <summary>
		/// Gets the URL to the media item. If HasValue is false returns an empty string.
		/// </summary>
		string Url { get; }

		/// <summary>
		/// Gets the ID of the media item the mediafield references
		/// </summary>
		ID MediaItemId { get; set; }

		/// <summary>
		/// The MediaItem that is the target of the file field
		/// </summary>
		MediaItem MediaItem { get; }

		/// <summary>
		/// Gets a stream of the binary data in the file field. Make sure to dispose of the object when done!
		/// </summary>
		MediaStream MediaStream { get; }
	}
}