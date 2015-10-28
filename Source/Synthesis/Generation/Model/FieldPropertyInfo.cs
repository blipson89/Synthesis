using Synthesis.FieldTypes;
using Synthesis.Templates;

namespace Synthesis.Generation.Model
{
	public class FieldPropertyInfo
	{
		private readonly ITemplateFieldInfo _field;

		public FieldPropertyInfo(ITemplateFieldInfo field)
		{
			_field = field;
		}

		public ITemplateFieldInfo Field { get { return _field; } }

		public FieldMapping FieldType { get; set; }
		public string SearchFieldName { get; set; }
		public string FieldPropertyName { get; set; }
	}
}
