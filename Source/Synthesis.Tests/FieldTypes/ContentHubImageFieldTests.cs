using Synthesis.Tests.Utility;
using Xunit;

namespace Synthesis.Tests.FieldTypes
{
    [Trait("Category", "FieldType Tests")]
    public class ContentHubImageFieldTests
	{
		public ContentHubImageFieldTests()
		{
			new FieldTestTemplateCreator().CreateSampleTemplate();
		}

		public void Disposable()
		{
			new FieldTestTemplateCreator().DeleteSampleTemplate();
		}
		
		[Fact(Skip = "TODO")]
		public void ContentHubImageField_GetContentId_ReturnsCorrectValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ContentHubImageField_GetContentId_ReturnsEmptyStringWhenBlank()
		{

		}

		[Fact(Skip = "TODO")]
		public void ContentHubImageField_SetContentId_SavesValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ContentHubImageField_GetThumbnailSrc_ReturnsCorrectValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ContentHubImageField_GetThumbnailSrc_ReturnsEmptyStringWhenBlank()
		{

		}

		[Fact(Skip = "TODO")]
		public void ContentHubImageField_SetThumbnailSrc_SavesValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ContentHubImageField_GetContentType_ReturnsCorrectValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ContentHubImageField_GetContentType_ReturnsEmptyStringWhenBlank()
		{

		}

		[Fact(Skip = "TODO")]
		public void ContentHubImageField_SetContentType_SavesValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ContentHubImageField_CanImplicitCastToContentHubImageField()
		{

		}

		[Fact(Skip = "TODO")]
		public void ContentHubImageField_HasValue_WhenTrue()
		{

		}

		[Fact(Skip = "TODO")]
		public void ContentHubImageField_HasValue_WhenFalse()
		{

		}
	}
}
