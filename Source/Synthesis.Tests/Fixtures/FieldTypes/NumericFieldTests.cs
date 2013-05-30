using System;
using NUnit.Framework;
using Sitecore.Data.Fields;
using Synthesis.FieldTypes;
using Synthesis.Tests.Utility;

namespace Synthesis.Tests.Fixtures.FieldTypes
{
	[TestFixture]
	[Category("FieldType Tests")]
	public class NumericFieldTests
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
		public void NumericField_GetValue_ReturnsValue()
		{

		}

		[Test, Ignore]
		public void NumericField_GetValue_ReturnsDefaultDecimalWhenBlank()
		{

		}

		[Test, Ignore]
		public void NumericField_SetValue_SavesValue()
		{

		}

		[Test, Ignore]
		public void NumericField_HasValue_WhenTrue()
		{

		}

		[Test, Ignore]
		public void NumericField_HasValue_WhenFalse()
		{

		}

		[Test]
		public void NumericField_CanImplicitCastToDecimal()
		{
			using (var item = new TestItemContext())
			{
				item.SetField(TestFields.NUMERIC, "3.14159");

				var field = new NumericField(new Lazy<Field>(() => item[TestFields.NUMERIC]), null);

				decimal value = field;

				Assert.AreEqual(value, 3.14159m);
			}
		}
	}
}
