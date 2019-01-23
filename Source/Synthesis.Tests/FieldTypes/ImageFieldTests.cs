using Synthesis.Tests.Utility;
using Xunit;

namespace Synthesis.Tests.FieldTypes
{
    [Trait("Category", "FieldType Tests")]
    public class ImageFieldTests
	{
		public ImageFieldTests()
		{
			new FieldTestTemplateCreator().CreateSampleTemplate();
		}

		public void Disposable()
		{
			new FieldTestTemplateCreator().DeleteSampleTemplate();
		}

		[Fact(Skip = "TODO")]
		public void ImageField_GetWidth_ReturnsCorrectValue()
		{

		}
		
		[Fact(Skip = "TODO")]
		public void ImageField_GetWidth_ReturnsNullIfBlank()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_SetWidth_SavesValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_SetWidth_RemovesValueWhenSavedAsNull()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_SetWidth_RemovesValueWhenSavedAsZero()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_GetHeight_ReturnsCorrectValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_GetHeight_ReturnsNullIfBlank()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_SetHeight_SavesValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_SetHeight_RemovesValueWhenSavedAsNull()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_SetHeight_RemovesValueWhenSavedAsZero()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_GetAlternateText_ReturnsCorrectValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_GetAlternateText_ReturnsEmptyStringWhenBlank()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_SetAlternateText_SavesValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_AttachAspImage_SetsInvisibleWhenNoValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_AttachAspImage_SetsImageUrl()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_AttachAspImage_SetsAlternateText()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_AttachAspImage_ScalesMaxWidth()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_AttachAspImage_ScalesMaxHeight()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_AttachImage_SetsInvisibleWhenNoValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_AttachImage_SetsItemAndField()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_AttachImage_ScalesMaxWidth()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_AttachImage_ScalesMaxHeight()
		{

		}

		[Fact(Skip = "TODO")]
		public void ImageField_CanImplicitCastToSitecoreImageField()
		{

		}
	}
}
