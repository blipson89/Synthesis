using NUnit.Framework;
using Synthesis.Tests.Utility;

namespace Synthesis.Tests.Fixtures.FieldTypes
{
	[TestFixture]
	[Category("FieldType Tests")]
	public class HyperlinkFieldTests
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
		public void HyperlinkField_HasValue_WhenTrue()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_HasValue_WhenFalse()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_GetTitle_ReturnsCorrectValue()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_GetTitle_ReturnsEmptyStringWhenBlank()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_SetTitle_SavesValue()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_GetTarget_ReturnsCorrectValue()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_GetTarget_ReturnsEmptyStringWhenBlank()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_SetTarget_SavesValue()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_GetCssClass_ReturnsCorrectValue()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_GetCssClass_ReturnsEmptyStringWhenBlank()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_SetCssClass_SavesValue()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_GetHref_ReturnsCorrectValue()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_GetHref_ReturnsEmptyStringWhenBlank()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_AttachHyperLink_SetsNavigateUrl()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_AttachHyperLink_SetsTarget()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_AttachHyperLink_SetsTitle()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_AttachHyperLink_SetsCssClass()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_AttachLink_SetsItemAndField()
		{

		}

		[Test, Ignore]
		public void HyperlinkField_CanImplicitCastToLinkField()
		{

		}
	}
}
