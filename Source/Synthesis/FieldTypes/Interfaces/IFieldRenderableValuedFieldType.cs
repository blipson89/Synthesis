namespace Synthesis.FieldTypes.Interfaces
{
	public interface IFieldRenderableValuedFieldType<TValue> : IFieldType, IFieldRenderableFieldType
	{
		TValue Value { get; set; }
	}
}
