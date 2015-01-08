using Synthesis.Templates;

namespace Synthesis.FieldTypes
{
	public interface IFieldMappingProvider
	{
		FieldMapping GetFieldType(ITemplateFieldInfo templateField);
	}
}
