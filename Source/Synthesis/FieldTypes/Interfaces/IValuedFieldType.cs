using System.Linq;

namespace Synthesis.FieldTypes.Interfaces
{
	public interface IValuedFieldType<out TValue> : IFieldType
	{
		TValue Value { get; }
	}
}
