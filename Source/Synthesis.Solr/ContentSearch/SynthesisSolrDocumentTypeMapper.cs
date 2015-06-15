using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch.SolrProvider.Mapping;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Methods;
using Sitecore.ContentSearch.Security;
using Sitecore.Diagnostics;
using Synthesis.ContentSearch;
using System.Collections;

namespace Synthesis.Solr.ContentSearch
{
	class SynthesisSolrDocumentTypeMapper : SolrDocumentPropertyMapper
	{
		private readonly SynthesisDocumentTypeMapper _synthesisMapper = new SynthesisDocumentTypeMapper();
		protected override object CreateElementInstance(System.Type baseType, IDictionary<string, object> fieldValues, IEnumerable<IExecutionContext> executionContexts)
		{
			return base.CreateElementInstance(baseType, fieldValues, executionContexts);
		}

		public override TElement MapToType<TElement>(Dictionary<string, object> document, SelectMethod selectMethod, IEnumerable<IFieldQueryTranslator> virtualFieldProcessors, IEnumerable<IExecutionContext> executionContexts, SearchSecurityOptions securityOptions)
		{
			if (document.ContainsKey("__smallupdateddate_tdt") && !document.ContainsKey("__smallupdateddate"))
			{
				document["__smallupdateddate"] = document["__smallupdateddate_tdt"];
				document.Remove("__smallupdateddate_tdt");
			}
			var mappingResult = _synthesisMapper.MapToType<TElement>(() => ExtractFieldsFromDocument(document, virtualFieldProcessors), selectMethod);

			// if the result type is not IStandardTemplateItem, use the default functionality
			if (!mappingResult.MappedSuccessfully)
				return base.MapToType<TElement>(document, selectMethod, virtualFieldProcessors, executionContexts, securityOptions);

			return mappingResult.Document;
		}

		private Dictionary<string, string> ExtractFieldsFromDocument(IDictionary<string, object> document, IEnumerable<IFieldQueryTranslator> virtualFieldProcessors)
		{
			Assert.ArgumentNotNull(document, "document");
			if (virtualFieldProcessors != null)
				document = virtualFieldProcessors.Aggregate(document, (current, processor) => processor.TranslateFieldResult(current, index.FieldNameTranslator));

			return document.ToDictionary(x => x.Key, x => ObjectToString(x.Value));
		}
		private static string ObjectToString(object value)
		{
			if (value is ArrayList)
			{
				return string.Join("|", from object obj in (ArrayList)value select obj);
			}
			return value.ToString();
		}

	}
}
