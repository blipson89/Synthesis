using Sitecore.Data;
using Sitecore.SecurityModel;

namespace Synthesis.Tests.Utility
{
	internal class FieldTestTemplateCreator
	{
		const string TEMPLATE_NAME = "Fake Template";

		internal void CreateSampleTemplate()
		{
			using (new SecurityDisabler())
			{
				var template = TemplateCreateUtility.CreateTestTemplate(TEMPLATE_NAME);

				var section = template.CreateTemplateSection("Test Fields");

				section.CreateTemplateField(TestFields.BOOLEAN, TemplateFieldTypes.CHECKBOX_FIELD);
				section.CreateTemplateField(TestFields.DATETIME, TemplateFieldTypes.DATETIME_FIELD);
				section.CreateTemplateField(TestFields.FILE, TemplateFieldTypes.FILE_FIELD);
				section.CreateTemplateField(TestFields.FILE_LIST, TemplateFieldTypes.FILE_DROP_AREA_FIELD);
				section.CreateTemplateField(TestFields.HYPERLINK, TemplateFieldTypes.GENERAL_LINK_FIELD);
				section.CreateTemplateField(TestFields.IMAGE, TemplateFieldTypes.IMAGE_FIELD);
				section.CreateTemplateField(TestFields.INTEGER, TemplateFieldTypes.INTEGER_FIELD);
				section.CreateTemplateField(TestFields.MULTIPLE_RELATION, TemplateFieldTypes.TREELIST_FIELD);
				section.CreateTemplateField(TestFields.NUMERIC, TemplateFieldTypes.NUMBER_FIELD);
				section.CreateTemplateField(TestFields.SINGLE_RELATION, TemplateFieldTypes.DROPLINK_FIELD);
				section.CreateTemplateField(TestFields.STRING, TemplateFieldTypes.TEXT_FIELD);
				section.CreateTemplateField(TestFields.WORD, TemplateFieldTypes.WORD_FIELD);

				CurrentTestTemplateID = template.ID;
			}
		}

		internal void DeleteSampleTemplate()
		{
			TemplateCreateUtility.DeleteTestTemplate(TEMPLATE_NAME);
			CurrentTestTemplateID = null;
		}

		internal static ID CurrentTestTemplateID { get; private set; }

		
	}
}
