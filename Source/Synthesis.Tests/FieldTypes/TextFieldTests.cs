using System;
using Synthesis.Tests.Utility;
using Xunit;

namespace Synthesis.Tests.FieldTypes
{
    [Trait("Category", "FieldType Tests")]
    public class TextFieldTests : IDisposable
	{
		public TextFieldTests()
		{
			new FieldTestTemplateCreator().CreateSampleTemplate();
		}

		public void Dispose()
		{
			new FieldTestTemplateCreator().DeleteSampleTemplate();
		}

		[Fact(Skip = "TODO")]
		public void TextField_GetRawValue_ReturnsValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void TextField_SetRawValue_SavesValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void TextField_HasValue_WhenTrue()
		{

		}

		[Fact(Skip = "TODO")]
		public void TextField_HasValue_WhenEmptyValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void TextField_HasValue_WhenWhitespaceValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void TextField_HasTextValue_WhenTrue()
		{

		}

		[Fact(Skip = "TODO")]
		public void TextField_HasTextValue_WhenEmptyValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void TextField_HasTextValue_WhenWhitespaceValue()
		{

		}
	}
}
