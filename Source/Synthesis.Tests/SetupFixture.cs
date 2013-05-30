using NUnit.Framework;
using Synthesis.Tests.Utility;

namespace Synthesis.Tests
{
	[SetUpFixture]
	public class SetupFixture
	{
		[SetUp]
		public void SetUpTests()
		{
			new FieldTestTemplateCreator().CreateSampleTemplate();
		}

		[TearDown]
		public void TearDownTests()
		{
			new FieldTestTemplateCreator().DeleteSampleTemplate();
			TemplateCreateUtility.CleanUpTestTemplatesFolder();
		}
	}
}
