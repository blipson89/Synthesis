using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Synthesis.Tests.Utility
{
    public static class TestingContants
    {
        const string TEMPLATE_PARENT_PATH = "/sitecore/Templates";
        public const string TEMPLATE_FOLDER = "SynTemp";

        public static string TestTemplatePath => TEMPLATE_PARENT_PATH + "/" + TEMPLATE_FOLDER;
        public static Item TestTemplateFolder => Factory.GetDatabase("master").GetItem(TestTemplatePath);
        public static ID TestTemplateFolderId = new ID("F24A9D1F-EEA9-4816-92DA-489EA06D1074");
    }
}
