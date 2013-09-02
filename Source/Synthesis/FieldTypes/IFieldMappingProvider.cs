using Sitecore.Data.Items;
namespace Synthesis.FieldTypes
{
	public interface IFieldMappingProvider
	{
		FieldMapping GetTemplateFieldType(TemplateFieldItem templateField);
		FieldMapping GetFieldType(string sitecoreFieldType);
	}
}
