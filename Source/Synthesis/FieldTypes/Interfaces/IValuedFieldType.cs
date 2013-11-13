using System.Linq;

namespace Synthesis.FieldTypes.Interfaces
{
	public interface IValuedFieldType<TValue> : IFieldType
	{
		TValue Value { get; set; }
	}
}
