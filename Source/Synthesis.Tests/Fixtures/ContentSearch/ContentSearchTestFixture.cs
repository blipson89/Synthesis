using System;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NSubstitute;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Maintenance;
using Sitecore.ContentSearch.Security;
using Sitecore.ContentSearch.SolrProvider;
using Sitecore.FakeDb;

namespace Synthesis.Tests.Fixtures.ContentSearch
{
	public class ContentSearchTestFixture : IDisposable
    {
        private Db database;
        private ConfigureServices _services;
        public ContentSearchTestFixture()
        {
            _services = new ConfigureServices();
            database = new Db("master");
            ContentSearchManager.SearchConfiguration.Indexes.Clear();
            ContentSearchManager.SearchConfiguration.Indexes.Add("sitecore_master_index", Substitute.For<ISearchIndex>());
        }

        public void Dispose()
        {
            _services.Dispose();
            database.Dispose();
        }
		protected IProviderSearchContext CreateTestSearchContext()
        {
			return ContentSearchManager.GetIndex("sitecore_master_index").CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck);
		}
	}
}
