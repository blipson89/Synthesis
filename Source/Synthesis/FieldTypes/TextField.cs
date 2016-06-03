using Sitecore.Web.UI.WebControls;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.FieldTypes
{
	public class TextField : FieldType, ITextField
	{
		public TextField(LazyField field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the raw value of the field. If the value is null an empty string will be returned.
		/// For rich text fields, this will not expand dynamic links in the content - use ExpandedLinksValue for that
		/// Setting this value will cause the underlying item to be loaded for search-based instances.
		/// </summary>
		public virtual string RawValue 
		{
			get
			{
				if(!IsFieldLoaded && InnerSearchValue != null) return InnerSearchValue;
				return InnerField?.Value;
			}
			set { SetFieldValue(value); }
		}

		/// <summary>
		/// Renders the field using a Sitecore FieldRenderer and returns the result
		/// Getting this value will cause the underlying item to be loaded for search-based instances.
		/// </summary>
		public virtual string RenderedValue => FieldRenderer.Render(InnerItem, InnerField.ID.ToString());

		/// <summary>
		/// Checks if the field has a null or empty value
		/// </summary>
		public override bool HasValue => !string.IsNullOrEmpty(RawValue);

		/// <summary>
		/// Checks if the field has a null, empty, or whitespace-only value
		/// </summary>
		public virtual bool HasTextValue => !string.IsNullOrWhiteSpace(RawValue);

		/// <summary>
		/// Converts the field into it's string representation. Returns the same value as the RawValue property.
		/// </summary>
		public override string ToString()
		{
			return RawValue;
		}
	}
}
