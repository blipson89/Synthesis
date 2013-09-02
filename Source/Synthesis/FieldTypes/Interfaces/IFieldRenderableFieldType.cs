
using System.Linq;

namespace Synthesis.FieldTypes.Interfaces
{
	/// <summary>
	/// Base interface for field types that are capable of being rendered using a FieldRenderer
	/// </summary>
	public interface IFieldRenderableFieldType
	{
		/// <summary>
		/// Renders the field using a Sitecore FieldRenderer and returns the result
		/// </summary>
		string RenderedValue { get; }
	}
}
