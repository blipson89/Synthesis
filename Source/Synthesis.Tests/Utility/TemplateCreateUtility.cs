using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Synthesis.Tests.Utility
{
	internal static class TemplateCreateUtility
	{
		const string TEMPLATE_PARENT_PATH = "/sitecore/Templates";
		const string TEMPLATE_FOLDER = "Temp";

		internal static string TestTemplatePath => TEMPLATE_PARENT_PATH + "/" + TEMPLATE_FOLDER;
		internal static Item TestTemplateFolder => Factory.GetDatabase("master").GetItem(TestTemplatePath);

		internal static Item CreateTestTemplate(string name)
		{
			using (new SecurityDisabler())
			{
				var parent = Factory.GetDatabase("master").GetItem(TEMPLATE_PARENT_PATH);
				var templateFolder = parent.Children[TEMPLATE_FOLDER];

				if (templateFolder == null) templateFolder = parent.Add(TEMPLATE_FOLDER, new TemplateID(TemplateIDs.TemplateFolder));

				return templateFolder.Add(name, new TemplateID(TemplateIDs.Template));
			}
		}

		internal static Item CreateTemplateSection(this Item template, string name)
		{
			using (new SecurityDisabler())
			{
				return template.Add(name, new TemplateID(TemplateIDs.TemplateSection));
			}
		}

		internal static Item CreateTemplateField(this Item templateSection, string name, string type)
		{
			using (new SecurityDisabler())
			{
				var field = templateSection.Add(name, new TemplateID(TemplateIDs.TemplateField));

				using (new EditContext(field))
				{
					field[TemplateFieldIDs.Type] = type;
				}

				return field;
			}
		}

		internal static void SetTemplateInheritance(this Item template, params Item[] baseTemplates)
		{
			using (new SecurityDisabler())
			{
				using (new EditContext(template))
				{
					template[FieldIDs.BaseTemplate] = string.Join("|", baseTemplates.Select(x => x.ID.ToString()).ToArray());
				}
			}
		}

		internal static void DeleteTestTemplate(string name)
		{
			var item = Factory.GetDatabase("master").GetItem(TEMPLATE_PARENT_PATH + "/" + name);

			if (item == null) return;

			using (new SecurityDisabler())
			{
				item.Delete();
			}
		}

		internal static void CleanUpTestTemplatesFolder()
		{
			using (new SecurityDisabler())
			{
				var item = Factory.GetDatabase("master").GetItem(TestTemplatePath);

				item?.Delete();
			}
		}
	}
}
