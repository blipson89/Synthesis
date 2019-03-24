using System;
using Synthesis.Tests.Utility;
using Xunit;

namespace Synthesis.Tests.FieldTypes
{
    [Trait("Category", "FieldType Tests")]
    public class NumericFieldTests : IDisposable
	{
		public NumericFieldTests()
		{
			new FieldTestTemplateCreator().CreateSampleTemplate();
		}

		public void Dispose()
		{
			new FieldTestTemplateCreator().DeleteSampleTemplate();
		}

		[Fact(Skip = "TODO")]
		public void NumericField_GetValue_ReturnsValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void NumericField_GetValue_ReturnsDefaultDecimalWhenBlank()
		{

		}

		[Fact(Skip = "TODO")]
		public void NumericField_SetValue_SavesValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void NumericField_HasValue_WhenTrue()
		{

		}

		[Fact(Skip = "TODO")]
		public void NumericField_HasValue_WhenFalse()
		{

		}
	}
}
