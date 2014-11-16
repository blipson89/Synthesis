using NUnit.Framework;
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
	}
}
