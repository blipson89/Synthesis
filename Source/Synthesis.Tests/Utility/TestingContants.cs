using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Synthesis.Tests.Utility
{
	internal static class TestingContants
	{
		const string TEMPLATE_PARENT_PATH = "/sitecore/Templates";
		public const string TEMPLATE_FOLDER = "SynTemp";

		internal static string TestTemplatePath =>  TEMPLATE_PARENT_PATH + "/" + TEMPLATE_FOLDER;
		internal static Item TestTemplateFolder => Factory.GetDatabase("master").GetItem(TestTemplatePath);
        internal static ID TestTemplateFolderId = new ID("F24A9D1F-EEA9-4816-92DA-489EA06D1074");
    }
}
