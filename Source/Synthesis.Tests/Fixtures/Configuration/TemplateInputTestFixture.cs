using System;
using System.Linq;
using NSubstitute;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Links;
using Sitecore.Links;
using Synthesis.Configuration;
using Synthesis.Tests.Utility;

namespace Synthesis.Tests.Fixtures.Configuration
{
    public class TemplateInputTestFixture : IDisposable
    {
        public DbTemplate AliceTemplate; // the alice template is a parent template with fields "Hello"
        public DbTemplate BobTemplate; // the bob template is a parent template with fields "Hi"
        public DbTemplate AllisonTemplate; // the allison template is  child template that inherits from alice and bob with fields "Child"
        public DbTemplate FlowerChildTemplate; // the flowerchild template inherits from the allison template with fields "Grandchild"
        
        public Db _db;
        private readonly LinkDatabase _linkDb = Substitute.For<LinkDatabase>();
        private readonly LinkDatabaseSwitcher _linkDbSwitcher;

        private static DbTemplate CreateTemplate(string name, string fieldName, string fieldType, ID[] baseTemplates = null)
        {
            var template = new DbTemplate($"{name}{Guid.NewGuid():N}")
            {
                ParentID = TestingContants.TestTemplateFolderId
            };
            template.Fields.Add(new DbField($"{fieldName}{Guid.NewGuid():N}") { Type = fieldType });
            if (baseTemplates != null)
                template.BaseIDs = baseTemplates;
            return template;
        }
        public TemplateInputTestFixture()
        {
            AliceTemplate = CreateTemplate("Alice", "Hello", TemplateFieldTypes.TEXT_FIELD);
            BobTemplate = CreateTemplate("Bob", "Hi", TemplateFieldTypes.TEXT_FIELD);
            AllisonTemplate = CreateTemplate("Allison", "Child", TemplateFieldTypes.TEXT_FIELD, new[] {AliceTemplate.ID, BobTemplate.ID});
            FlowerChildTemplate = CreateTemplate("FlowerChild", "Grandchild", TemplateFieldTypes.TEXT_FIELD, new[] {AllisonTemplate.ID});
            
            _db = new Db("master")
            {
                new DbTemplate(TemplateIDs.TemplateFolder),
                new DbItem(TestingContants.TEMPLATE_FOLDER, TestingContants.TestTemplateFolderId) { ParentID = ItemIDs.TemplateRoot, TemplateID = TemplateIDs.TemplateFolder},
                AliceTemplate,
                BobTemplate,
                AllisonTemplate,
                FlowerChildTemplate
            };

            _linkDbSwitcher = new LinkDatabaseSwitcher(_linkDb);
            var templateFieldTemplate = new TemplateItem(_db.GetItem(TemplateIDs.TemplateField));
            _linkDb.GetReferrers(templateFieldTemplate.InnerItem).Returns(new[]
            {
                MakeLink(AliceTemplate),
                MakeLink(BobTemplate),
                MakeLink(AllisonTemplate),
                MakeLink(FlowerChildTemplate)
            });
        }

        private ItemLink MakeLink(DbItem template)
        {
            var field = _db.GetItem(template.Fields.First().ID);
            return new ItemLink(field, ID.NewID, new Item(ID.NewID, ItemData.Empty, _db.Database), field.Paths.FullPath);
        }
        public string ParentFullPath(DbTemplate template)
        {
            return _db.GetItem(template.ParentID).Paths.FullPath;
        }
        // gets a provider that will return all four test templates unless exclusions are applied
        public ConfigurationTemplateInputProvider GetExclusionTestProvider()
        {
            var provider = new ConfigurationTemplateInputProvider();
            provider.AddTemplatePath(TestingContants.TestTemplatePath);
            return provider;
        }

        public void Dispose()
        {
            _linkDbSwitcher.Dispose();
            _db.Dispose();
        }
    }
}