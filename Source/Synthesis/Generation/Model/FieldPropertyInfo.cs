using Synthesis.FieldTypes;
using Synthesis.Templates;

namespace Synthesis.Generation.Model
{
	public class FieldPropertyInfo
	{
		public FieldPropertyInfo(ITemplateFieldInfo field)
		{
			Field = field;
		}

		public ITemplateFieldInfo Field { get; }

		public FieldMapping FieldType { get; set; }
		public string SearchFieldName { get; set; }
		public string FieldPropertyName { get; set; }
	}
}
