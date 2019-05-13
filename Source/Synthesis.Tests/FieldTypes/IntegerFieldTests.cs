using System;
using Synthesis.Tests.Utility;
using Xunit;

namespace Synthesis.Tests.FieldTypes
{
    [Trait("Category", "FieldType Tests")]
    public class IntegerFieldTests : IDisposable
	{
		public IntegerFieldTests()
		{
			new FieldTestTemplateCreator().CreateSampleTemplate();
		}

		public void Dispose()
		{
			new FieldTestTemplateCreator().DeleteSampleTemplate();
		}

		[Fact(Skip = "TODO")]
		public void IntegerField_GetValue_ReturnsValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void IntegerField_GetValue_ReturnsDefaultIntWhenBlank()
		{

		}

		[Fact(Skip = "TODO")]
		public void IntegerField_SetValue_SavesValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void IntegerField_HasValue_WhenTrue()
		{

		}

		[Fact(Skip = "TODO")]
		public void IntegerField_HasValue_WhenFalse()
		{

		}
	}
}
