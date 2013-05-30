using NUnit.Framework;
using Synthesis.Tests.Utility;

namespace Synthesis.Tests.Fixtures.FieldTypes
{
	[TestFixture]
	[Category("FieldType Tests")]
	public class FileFieldTests
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
		public void FileField_GetUrl_IsResultValid()
		{

		}

		[Test, Ignore]
		public void FileField_GetUrl_ReturnsEmptyStringWhenNoFile()
		{

		}

		[Test, Ignore]
		public void FileField_GetMediaItemID_ValidMediaItem()
		{

		}

		[Test, Ignore]
		public void FileField_GetMediaItemID_InvalidMediaItem()
		{

		}

		[Test, Ignore]
		public void FileField_SetMediaItemID_SavesValue()
		{

		}

		[Test, Ignore]
		public void FileField_HasValue_WhenTrue()
		{

		}

		[Test, Ignore]
		public void FileField_HasValue_WhenFalse()
		{

		}

		[Test, Ignore]
		public void FileField_AttachLink_SetsInvisibleWhenNoValue()
		{

		}

		[Test, Ignore]
		public void FileField_AttachLink_SetsCorrectUrl()
		{

		}

		[Test, Ignore]
		public void FileField_RenderLink_RendersNothingWhenNoValue()
		{

		}

		[Test, Ignore]
		public void FileField_RenderLink_RendersCorrectLink()
		{

		}

		[Test, Ignore]
		public void FileField_CanImplicitCastToSitecoreFileField()
		{

		}
	}
}
