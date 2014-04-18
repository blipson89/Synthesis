using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Security;

namespace Synthesis.Tests.Fixtures.ContentSearch
{
	public class ContentSearchTestFixture
	{
		protected IProviderSearchContext CreateTestSearchContext()
		{
			return ContentSearchManager.GetIndex("sitecore_master_index").CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck);
		}
	}
}
