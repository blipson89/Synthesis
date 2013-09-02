using System;
using NUnit.Framework;
using Sitecore.Data.Fields;
using Sitecore.SecurityModel;
using Synthesis.FieldTypes;
using Synthesis.Tests.Utility;

namespace Synthesis.Tests.Fixtures.FieldTypes.FieldTypes
{
	[TestFixture]
	[Category("FieldType Tests")]
	public class BooleanFieldTests
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

		[Test]
		public void BooleanField_GetValue_WhenTrue()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.BOOLEAN, "1");

				var field = new BooleanField(new Lazy<Field>(() => item[TestFields.BOOLEAN]), null);
				
				Assert.IsTrue(field.Value);
			}
		}

		[Test]
		public void BooleanField_GetValue_ReturnsFalseWhenBlank()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.BOOLEAN, "");

				var field = new BooleanField(new Lazy<Field>(() => item[TestFields.BOOLEAN]), null);

				Assert.IsFalse(field.Value);
			}
		}

		[Test]
		public void BooleanField_GetValue_ReturnsFalseWhenZero()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.BOOLEAN, "0");

				var field = new BooleanField(new Lazy<Field>(() => item[TestFields.BOOLEAN]), null);

				Assert.IsFalse(field.Value);
			}
		}

		[Test]
		public void BooleanField_SetValue_SavesWhenSetTrue()
		{
			using (var item = new TestItemContext())
			{
				var field = new BooleanField(new Lazy<Field>(() => item[TestFields.BOOLEAN]), null);

				using (new SecurityDisabler())
				{
					field.Value = true;
				}

				CheckboxField sitecoreField = item[TestFields.BOOLEAN];

				Assert.IsTrue(sitecoreField.Checked);
			}
		}

		[Test]
		public void BooleanField_SetValue_SavesWhenSetFalse()
		{
			using (var item = new TestItemContext())
			{
				var field = new BooleanField(new Lazy<Field>(() => item[TestFields.BOOLEAN]), null);

				using (new SecurityDisabler())
				{
					field.Value = false;
				}

				CheckboxField sitecoreField = item[TestFields.BOOLEAN];

				Assert.IsFalse(sitecoreField.Checked);
			}
		}
	}
}
