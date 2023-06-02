using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Sitecore;
using Sitecore.ContentSearch.Abstractions;
using Sitecore.ContentSearch.SolrProvider;
using Sitecore.ContentSearch.SolrProvider.FieldNames;
using Sitecore.ContentSearch.SolrProvider.FieldNames.Normalization;
using Sitecore.ContentSearch.SolrProvider.FieldNames.TypeResolving;
using SolrNet.Schema;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Solr.ContentSearch
{
    public class SynthesisSolrFieldNameTranslator : SolrFieldNameTranslator
    {

        private readonly SolrSchema _schema;

        public SynthesisSolrFieldNameTranslator(SolrFieldMap solrFieldMap, SolrIndexSchema solrIndexSchema,
            ISettings settings, ISolrFieldConfigurationResolver fieldConfigurationResolver,
            IExtensionStripHelper extensionStripHelper, TemplateFieldTypeResolverFactory typeResolverFactory,
            ICultureContextGuard cultureContextGuard)
            : base(solrFieldMap, solrIndexSchema, settings, fieldConfigurationResolver, extensionStripHelper,
                typeResolverFactory, cultureContextGuard)
        {
            _schema = solrIndexSchema.SolrSchema;
        }


        public override string GetIndexFieldName(string fieldName)
        {
            if (_schema != null && (_schema.FindSolrFieldByName(fieldName) != null || _schema.SolrDynamicFields.Any(x => fieldName.EndsWith(x.Name.Substring(1)))))
                return fieldName;
            
            // Let default Sitecore API try to resolve it
            return base.GetIndexFieldName(fieldName, (CultureInfo)null);
        }

        public override string GetIndexFieldName(MemberInfo member)
        {
            // If it's not a synthesis field type, let Sitecore handle it.
            if (!(member is PropertyInfo p) || !typeof(IFieldType).IsAssignableFrom(p.PropertyType))
                return base.GetIndexFieldName(member);

            string name = GetIndexFieldNameFormatterAttribute(member)?.GetIndexFieldName(member.Name);
            return !string.IsNullOrEmpty(name) ? name : base.GetIndexFieldName(member);
        }

        public override string GetIndexFieldName(string fieldName, Type returnType)
        {
            string name = PreProcessSynthesisFieldName(fieldName);

            return typeof(IFieldType).IsAssignableFrom(returnType) ? name : base.GetIndexFieldName(name, returnType);
        }

        protected virtual string PreProcessSynthesisFieldName(string fieldName)
        {
            return fieldName.Split('.').First();
        }

        /// <summary>
        /// If the context is a foreign language we should use the foreign language text solr fields
        /// </summary>
        /// <param name="fieldName">the initial field name</param>
        /// <returns>field name with a dynamic field identifier on it</returns>
        private string AppendSolrText(string fieldName)
        {
            if (Context.Site == null || Context.Language.Name == Context.Site.Language)
                fieldName += "_t";
            else
                fieldName += "_t_" + Context.Language;
            return fieldName;
        }
    }
}
