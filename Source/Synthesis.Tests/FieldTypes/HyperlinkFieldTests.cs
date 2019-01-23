using System;
using Synthesis.Tests.Utility;
using Xunit;

namespace Synthesis.Tests.FieldTypes
{

    [Trait("Category", "FieldType Tests")]
    public class HyperlinkFieldTests : IDisposable
	{
		public HyperlinkFieldTests()
		{
			new FieldTestTemplateCreator().CreateSampleTemplate();
		}

		public void Dispose()
		{
			new FieldTestTemplateCreator().DeleteSampleTemplate();
		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_HasValue_WhenTrue()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_HasValue_WhenFalse()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_GetTitle_ReturnsCorrectValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_GetTitle_ReturnsEmptyStringWhenBlank()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_SetTitle_SavesValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_GetTarget_ReturnsCorrectValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_GetTarget_ReturnsEmptyStringWhenBlank()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_SetTarget_SavesValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_GetCssClass_ReturnsCorrectValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_GetCssClass_ReturnsEmptyStringWhenBlank()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_SetCssClass_SavesValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_GetHref_ReturnsCorrectValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_GetHref_ReturnsEmptyStringWhenBlank()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_AttachHyperLink_SetsNavigateUrl()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_AttachHyperLink_SetsTarget()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_AttachHyperLink_SetsTitle()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_AttachHyperLink_SetsCssClass()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_AttachLink_SetsItemAndField()
		{

		}

		[Fact(Skip = "TODO")]
		public void HyperlinkField_CanImplicitCastToLinkField()
		{

		}
	}
}
