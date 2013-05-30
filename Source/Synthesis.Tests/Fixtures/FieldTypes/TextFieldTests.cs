using System;
using NUnit.Framework;
using Sitecore.Data.Fields;
using Synthesis.Tests.Utility;
using TextField = Synthesis.FieldTypes.TextField;

namespace Synthesis.Tests.Fixtures.FieldTypes
{
	[TestFixture]
	[Category("FieldType Tests")]
	public class TextFieldTests
	{
		[TestFixtureSetUp]
		public void SetUpTestTemplate()
		{
			new FieldTestTemplateCreator().CreateSampleTemplate();
		}

		[TestFixtureTearDown]
		public void TearDownTestTemplate()
		{
			new FieldTestTemplateCreator().DeleteSampleTemplate();
		}

		[Test, Ignore]
		public void TextField_GetRawValue_ReturnsValue()
		{

		}

		[Test, Ignore]
		public void TextField_SetRawValue_SavesValue()
		{

		}

		[Test, Ignore]
		public void TextField_HasValue_WhenTrue()
		{

		}

		[Test, Ignore]
		public void TextField_HasValue_WhenEmptyValue()
		{

		}

		[Test, Ignore]
		public void TextField_HasValue_WhenWhitespaceValue()
		{

		}

		[Test, Ignore]
		public void TextField_HasTextValue_WhenTrue()
		{

		}

		[Test, Ignore]
		public void TextField_HasTextValue_WhenEmptyValue()
		{

		}

		[Test, Ignore]
		public void TextField_HasTextValue_WhenWhitespaceValue()
		{

		}

		[Test]
		public void TextField_CanImplicitCastToString()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.STRING, "the Monstrous Blue Whale #1");

				var field = new TextField(new Lazy<Field>(() => item[TestFields.STRING]), null);

				string value = field;

				Assert.AreEqual(value, "the Monstrous Blue Whale #1");
			}
		}
	}
}
