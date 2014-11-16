using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Testing
{
	public class TestTextField : TestFieldType, ITextField
	{
		public TestTextField(string value)
		{
			RawValue = value;
		}

		/// <summary>
		///     Gets the raw value of the field. If the value is null an empty string will be returned.
		///     For rich text fields, this will not expand dynamic links in the content - use ExpandedLinksValue for that
		///     Setting this value will cause the underlying item to be loaded for search-based instances.
		/// </summary>
		public string RawValue { get; set; }

		/// <summary>
		///     Renders the field using a Sitecore FieldRenderer and returns the result
		///     Getting this value will cause the underlying item to be loaded for search-based instances.
		/// </summary>
		public virtual string RenderedValue
		{
			get { return RawValue; }
		}

		/// <summary>
		///     Checks if the field has a null or empty value
		/// </summary>
		public override bool HasValue
		{
			get { return !string.IsNullOrEmpty(RawValue); }
		}

		/// <summary>
		///     Checks if the field has a null, empty, or whitespace-only value
		/// </summary>
		public virtual bool HasTextValue
		{
			get { return !string.IsNullOrWhiteSpace(RawValue); }
		}
	}
}