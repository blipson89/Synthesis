using System;
using Sitecore.Data.Fields;
using Synthesis.FieldTypes.Interfaces;
using Synthesis.Utility;

namespace Synthesis.FieldTypes
{
	/// <summary>
	/// Represents a field whose contents are HTML written by a rich text editor that may contain dynamic links
	/// </summary>
	public class RichTextField : TextField, IRichTextField
	{
		public RichTextField(Lazy<Field> field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the raw value of the field with dynamic links expanded into friendly URLs. Unlike RenderedValue this does not support Page Editor, but neither does it require loading a search-based instance's underlying item like RenderedValue.
		/// </summary>
		public virtual string ExpandedLinksValue
		{
			get { return FieldUtility.ExpandDynamicLinks(RawValue); }
		}

		/// <summary>
		/// Converts the field into it's string representation. Returns the same value as the ExpandedLinksValue property.
		/// </summary>
		public override string ToString()
		{
			return ExpandedLinksValue;
		}
	}
}
