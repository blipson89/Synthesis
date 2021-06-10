using System;
using AutoFixture;
using NSubstitute;
using Sitecore;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.SecurityModel;

namespace Synthesis.Tests.Utility
{
	internal class FieldTestTemplateCreator : IDisposable
	{
        private readonly Db _database;
        private readonly IFixture _fixture;
        public FieldTestTemplateCreator()
        {
            _fixture = new Fixture();
            var templateFolder = new DbItem(_fixture.Create<string>(), ID.NewID)
            {
                ParentID = ItemIDs.TemplateRoot,
                TemplateID = TemplateIDs.TemplateFolder
            };
            _database = new Db("master")
            {
                new DbTemplate(CurrentTestTemplateID),
                new DbTemplate(TemplateIDs.TemplateFolder),
                templateFolder,
                CreateSampleTemplate()
            };
        }
		internal DbTemplate CreateSampleTemplate()
		{
			using (new SecurityDisabler())
			{
				var template = new DbTemplate();

				CreateTemplateField(template, TestFields.BOOLEAN, TemplateFieldTypes.CHECKBOX_FIELD);
				CreateTemplateField(template, TestFields.DATETIME, TemplateFieldTypes.DATETIME_FIELD);
				CreateTemplateField(template, TestFields.FILE, TemplateFieldTypes.FILE_FIELD);
				CreateTemplateField(template, TestFields.HYPERLINK, TemplateFieldTypes.GENERAL_LINK_FIELD);
				CreateTemplateField(template, TestFields.IMAGE, TemplateFieldTypes.IMAGE_FIELD);
                CreateTemplateField(template, TestFields.CONTENT_HUB_IMAGE, TemplateFieldTypes.CONTENT_HUB_IMAGE_FIELD);
				CreateTemplateField(template, TestFields.INTEGER, TemplateFieldTypes.INTEGER_FIELD);
				CreateTemplateField(template, TestFields.MULTIPLE_RELATION, TemplateFieldTypes.TREELIST_FIELD);
				CreateTemplateField(template, TestFields.NUMERIC, TemplateFieldTypes.NUMBER_FIELD);
				CreateTemplateField(template, TestFields.SINGLE_RELATION, TemplateFieldTypes.DROPLINK_FIELD);
				CreateTemplateField(template, TestFields.TEXT, TemplateFieldTypes.TEXT_FIELD);

				CurrentTestTemplateID = template.ID;

                return template;
            }
		}
        internal DbField CreateTemplateField(DbTemplate template, string name, string type)
        {
            var field = new DbField(name) { Type = type };
            template.Fields.Add(field);
            return field;
        }

        internal void DeleteSampleTemplate()
        {

        }
		internal static ID CurrentTestTemplateID { get; private set; }

        public void Dispose()
        {
            CurrentTestTemplateID = null;
            _database.Dispose();
        }
	}
}
