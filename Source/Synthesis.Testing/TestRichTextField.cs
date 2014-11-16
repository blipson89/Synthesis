using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Testing
{
	/// <summary>
	///     Represents a field whose contents are HTML written by a rich text editor that may contain dynamic links
	/// </summary>
	public class TestRichTextField : TestTextField, IRichTextField
	{
		public TestRichTextField(string value) : base(value)
		{
		}

		/// <summary>
		///     Gets the raw value of the field with dynamic links expanded into friendly URLs. Unlike RenderedValue this does not support Page Editor, but neither does it require loading a search-based instance's underlying item like RenderedValue.
		/// </summary>
		public virtual string ExpandedLinksValue
		{
			get { return RawValue; }
		}
	}
}