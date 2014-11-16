using NUnit.Framework;
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
	}
}
