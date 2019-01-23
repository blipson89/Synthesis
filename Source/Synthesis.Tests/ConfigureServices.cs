using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Synthesis.ContentSearch;
using Synthesis.Solr.ContentSearch;

namespace Synthesis.Tests
{
    public class ConfigureServices : IDisposable
    {
        private readonly IServiceScope _scope;
        private readonly IServiceProvider _defaultProvider;
        public ConfigureServices()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            new DefaultSitecoreServicesConfigurator().Configure(serviceCollection);
            Configure(serviceCollection);

            _scope = serviceCollection.BuildServiceProvider().CreateScope();
            _defaultProvider = ServiceLocator.ServiceProvider;
            ServiceLocator.SetServiceProvider(_scope.ServiceProvider);
        }
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IQueryableResolver, ResolveSolrQueryable>();
            serviceCollection.AddTransient<IFieldNameTranslatorFactory, SynthesisSolrFieldNameTranslatorFactory>();
        }

        public void Dispose()
        {
            ServiceLocator.SetServiceProvider(_defaultProvider);
            _scope.Dispose();
        }
    }
}
