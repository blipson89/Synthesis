using System;
using Synthesis.Tests.Utility;
using Xunit;

namespace Synthesis.Tests.FieldTypes
{
    [Trait("Category", "FieldType Tests")]
    public class FileFieldTests : IDisposable
	{
		public FileFieldTests()
		{
			new FieldTestTemplateCreator().CreateSampleTemplate();
		}

		public void Dispose()
		{
			new FieldTestTemplateCreator().DeleteSampleTemplate();
		}

		[Fact(Skip = "TODO")]
		public void FileField_GetUrl_IsResultValid()
		{

		}

		[Fact(Skip = "TODO")]
		public void FileField_GetUrl_ReturnsEmptyStringWhenNoFile()
		{

		}

		[Fact(Skip = "TODO")]
		public void FileField_GetMediaItemID_ValidMediaItem()
		{

		}

		[Fact(Skip = "TODO")]
		public void FileField_GetMediaItemID_InvalidMediaItem()
		{

		}

		[Fact(Skip = "TODO")]
		public void FileField_SetMediaItemID_SavesValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void FileField_HasValue_WhenTrue()
		{

		}

		[Fact(Skip = "TODO")]
		public void FileField_HasValue_WhenFalse()
		{

		}

		[Fact(Skip = "TODO")]
		public void FileField_AttachLink_SetsInvisibleWhenNoValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void FileField_AttachLink_SetsCorrectUrl()
		{

		}

		[Fact(Skip = "TODO")]
		public void FileField_RenderLink_RendersNothingWhenNoValue()
		{

		}

		[Fact(Skip = "TODO")]
		public void FileField_RenderLink_RendersCorrectLink()
		{

		}

		[Fact(Skip = "TODO")]
		public void FileField_CanImplicitCastToSitecoreFileField()
		{

		}
	}
}
