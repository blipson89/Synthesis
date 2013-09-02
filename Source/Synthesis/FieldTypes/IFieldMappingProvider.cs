using Sitecore.Data.Items;
namespace Synthesis.FieldTypes
{
	public interface IFieldMappingProvider
	{
		FieldMapping GetFieldType(TemplateFieldItem templateField);
	}
}
