using System.Linq;

namespace Synthesis.FieldTypes.Interfaces
{
	public interface IFieldRenderableValuedFieldType<out TValue> : IFieldType, IFieldRenderableFieldType
	{
		TValue Value { get; }
	}
}
