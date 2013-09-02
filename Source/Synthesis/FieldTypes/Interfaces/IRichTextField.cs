using System.Linq;

namespace Synthesis.FieldTypes.Interfaces
{
	public interface IRichTextField : ITextField
	{
		/// <summary>
		/// Gets the raw value of the field with dynamic links expanded into friendly URLs. Unlike RenderedValue this does not support Page Editor, but neither does it require loading a search-based instance's underlying item like RenderedValue.
		/// </summary>
		string ExpandedLinksValue { get; }
	}
}