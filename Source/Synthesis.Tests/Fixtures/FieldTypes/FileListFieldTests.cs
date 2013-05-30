using NUnit.Framework;
using Synthesis.Tests.Utility;

namespace Synthesis.Tests.Fixtures.FieldTypes
{
	[TestFixture]
	[Category("FieldType Tests")]
	public class FileListFieldTests
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
		public void FileListField_GetMediaItems_ReturnsCorrectItems()
		{

		}

		[Test, Ignore]
		public void FileListField_GetMediaUrls_ReturnsCorrectUrls()
		{

		}

		[Test, Ignore]
		public void FileListField_HasValue_WhenTrue()
		{

		}

		[Test, Ignore]
		public void FileListField_HasValue_WhenFalse()
		{

		}

		[Test, Ignore]
		public void FileListField_CanImplicitCastToFileDropAreaField()
		{

		}
	}
}
