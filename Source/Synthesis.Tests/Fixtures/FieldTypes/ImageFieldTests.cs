using NUnit.Framework;
using Synthesis.Tests.Utility;

namespace Synthesis.Tests.Fixtures.FieldTypes
{
	[TestFixture]
	[Category("FieldType Tests")]
	public class ImageFieldTests
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
		public void ImageField_GetWidth_ReturnsCorrectValue()
		{

		}
		
		[Test, Ignore]
		public void ImageField_GetWidth_ReturnsNullIfBlank()
		{

		}

		[Test, Ignore]
		public void ImageField_SetWidth_SavesValue()
		{

		}

		[Test, Ignore]
		public void ImageField_SetWidth_RemovesValueWhenSavedAsNull()
		{

		}

		[Test, Ignore]
		public void ImageField_SetWidth_RemovesValueWhenSavedAsZero()
		{

		}

		[Test, Ignore]
		public void ImageField_GetHeight_ReturnsCorrectValue()
		{

		}

		[Test, Ignore]
		public void ImageField_GetHeight_ReturnsNullIfBlank()
		{

		}

		[Test, Ignore]
		public void ImageField_SetHeight_SavesValue()
		{

		}

		[Test, Ignore]
		public void ImageField_SetHeight_RemovesValueWhenSavedAsNull()
		{

		}

		[Test, Ignore]
		public void ImageField_SetHeight_RemovesValueWhenSavedAsZero()
		{

		}

		[Test, Ignore]
		public void ImageField_GetAlternateText_ReturnsCorrectValue()
		{

		}

		[Test, Ignore]
		public void ImageField_GetAlternateText_ReturnsEmptyStringWhenBlank()
		{

		}

		[Test, Ignore]
		public void ImageField_SetAlternateText_SavesValue()
		{

		}

		[Test, Ignore]
		public void ImageField_AttachAspImage_SetsInvisibleWhenNoValue()
		{

		}

		[Test, Ignore]
		public void ImageField_AttachAspImage_SetsImageUrl()
		{

		}

		[Test, Ignore]
		public void ImageField_AttachAspImage_SetsAlternateText()
		{

		}

		[Test, Ignore]
		public void ImageField_AttachAspImage_ScalesMaxWidth()
		{

		}

		[Test, Ignore]
		public void ImageField_AttachAspImage_ScalesMaxHeight()
		{

		}

		[Test, Ignore]
		public void ImageField_AttachImage_SetsInvisibleWhenNoValue()
		{

		}

		[Test, Ignore]
		public void ImageField_AttachImage_SetsItemAndField()
		{

		}

		[Test, Ignore]
		public void ImageField_AttachImage_ScalesMaxWidth()
		{

		}

		[Test, Ignore]
		public void ImageField_AttachImage_ScalesMaxHeight()
		{

		}

		[Test, Ignore]
		public void ImageField_CanImplicitCastToSitecoreImageField()
		{

		}
	}
}
