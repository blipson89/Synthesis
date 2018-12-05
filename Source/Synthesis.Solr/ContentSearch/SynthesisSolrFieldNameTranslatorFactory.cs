using System.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Abstractions;
using Sitecore.ContentSearch.SolrProvider;
using Sitecore.ContentSearch.SolrProvider.FieldNames;
using Sitecore.ContentSearch.SolrProvider.FieldNames.Normalization;
using Sitecore.ContentSearch.SolrProvider.FieldNames.TypeResolving;
using Sitecore.Diagnostics;
using Synthesis.ContentSearch;
using Synthesis.Utility;

namespace Synthesis.Solr.ContentSearch
{
    public class SynthesisSolrFieldNameTranslatorFactory : IFieldNameTranslatorFactory
    {
        public AbstractFieldNameTranslator GetFieldNameTranslator(ISearchIndex index)
        {
            SolrFieldMap fieldMap = index.Configuration.FieldMap as SolrFieldMap;
            SolrIndexSchema schema = index.Schema as SolrIndexSchema;
            ISettings instance = index.Locator.GetInstance<ISettings>();
            SolrIndexConfiguration configuration = index.Configuration as SolrIndexConfiguration;
            Assert.IsNotNull(fieldMap, "FieldMap is null.");
            Assert.IsNotNull(schema, "SolrSchema is null.");
            Assert.IsNotNull(instance, "Settings is null.");
            Assert.IsNotNull(configuration, "This constructor requires SolrIndexConfiguration.");
            TemplateFieldTypeResolverFactory typeResolverFactory = configuration.TemplateFieldTypeResolverFactory;
            Assert.IsNotNull(typeResolverFactory, "normalizerFactory is null.");
            TemplateFieldTypeResolver fieldTypeResolver = typeResolverFactory.Create();
            Assert.IsNotNull(fieldTypeResolver, "FieldTypeResolver is null.");
            SolrFieldConfigurationResolver configurationResolver = new SolrFieldConfigurationResolver(fieldMap, schema, fieldTypeResolver);
            var ctor = typeof(SolrFieldNameTranslator).Assembly
                .GetType("Sitecore.ContentSearch.SolrProvider.FieldNames.CultureContextGuard")
                .GetConstructors().First();


            ICultureContextGuard cultureContextGuard = ObjectActivator.GetActivator<ICultureContextGuard>(ctor).Invoke();
            return new SynthesisSolrFieldNameTranslator(fieldMap, schema, instance, configurationResolver, new ExtensionStripHelper(fieldMap, schema), typeResolverFactory, cultureContextGuard);
        }
    }
}