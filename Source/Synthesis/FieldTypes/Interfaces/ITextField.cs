namespace Synthesis.FieldTypes.Interfaces
{
	public interface ITextField : IFieldRenderableFieldType, IFieldType
	{
		/// <summary>
		/// Gets the raw value of the field. If the value is null an empty string will be returned.
		/// For rich text fields, this will not expand dynamic links in the content - use ExpandedLinksValue for that
		/// Setting this value will cause the underlying item to be loaded for search-based instances.
		/// </summary>
		string RawValue { get; set; }

		/// <summary>
		/// Checks if the field has a null, empty, or whitespace-only value
		/// </summary>
		bool HasTextValue { get; }
	}
}