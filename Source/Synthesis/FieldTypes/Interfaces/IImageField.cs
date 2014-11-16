namespace Synthesis.FieldTypes.Interfaces
{
	public interface IImageField : IFileField, IFieldRenderableFieldType
	{
		/// <summary>
		/// Gets the width of the image, if one was entered
		/// </summary>
		int? Width { get; set; }

		/// <summary>
		/// Gets the height of the image, if one was entered
		/// </summary>
		int? Height { get; set; }

		/// <summary>
		/// Gets the alt text of the image, if any was entered
		/// </summary>
		string AlternateText { get; set; }
	}
}