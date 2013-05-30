using System;
using NUnit.Framework;
using Sitecore.Data.Fields;
using Synthesis.FieldTypes;
using Synthesis.Tests.Utility;

namespace Synthesis.Tests.Fixtures.FieldTypes
{
	[TestFixture]
	[Category("FieldType Tests")]
	public class IntegerFieldTests
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
		public void IntegerField_GetValue_ReturnsValue()
		{

		}

		[Test, Ignore]
		public void IntegerField_GetValue_ReturnsDefaultIntWhenBlank()
		{

		}

		[Test, Ignore]
		public void IntegerField_SetValue_SavesValue()
		{

		}

		[Test, Ignore]
		public void IntegerField_HasValue_WhenTrue()
		{

		}

		[Test, Ignore]
		public void IntegerField_HasValue_WhenFalse()
		{

		}

		[Test]
		public void IntegerField_CanImplicitCastToInt()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.INTEGER, "16");

				var field = new IntegerField(new Lazy<Field>(() => item[TestFields.INTEGER]), null);

				int value = field;

				Assert.AreEqual(value, 16);
			}
		}
	}
}
