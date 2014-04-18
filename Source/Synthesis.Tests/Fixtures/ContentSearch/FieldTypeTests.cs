using NUnit.Framework;

namespace Synthesis.Tests.Fixtures.ContentSearch
{
	/*
	 * ASSUMPTIONS OF THESE TESTS:
	 * Standard sitecore_master_index configured
	 * All items in master are allowed to be indexed
	 * The _system_ templates are relatively unmodified (used as query playground)
	 */
	[TestFixture]
	[Category("Content Search Tests")]
	public class FieldTypeTests : ContentSearchTestFixture
	{
		[Test]
		public void BooleanField_SearchByValue_FindsItem()
		{
			using (var context = CreateTestSearchContext())
			{
				
			}
		}
	}
}
