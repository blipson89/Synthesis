using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch.SolrProvider.Mapping;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Methods;
using Sitecore.ContentSearch.Security;
using Sitecore.Diagnostics;
using Synthesis.ContentSearch;
using System.Collections;
using System.Text;

namespace Synthesis.Solr.ContentSearch
{
	public class SynthesisSolrDocumentTypeMapper : SolrDocumentPropertyMapper
	{
		private readonly SynthesisDocumentTypeMapper _synthesisMapper = new SynthesisDocumentTypeMapper();
		public override TElement MapToType<TElement>(Dictionary<string, object> document, SelectMethod selectMethod, IEnumerable<IFieldQueryTranslator> virtualFieldProcessors, IEnumerable<IExecutionContext> executionContexts, SearchSecurityOptions securityOptions)
		{
			ConvertDocumentToSythesisStandardTemplate(document);
			var mappingResult = _synthesisMapper.MapToType<TElement>(() => ExtractFieldsFromDocument(document, virtualFieldProcessors), selectMethod);

			// if the result type is not IStandardTemplateItem, use the default functionality
			if (!mappingResult.MappedSuccessfully)
				return base.MapToType<TElement>(document, selectMethod, virtualFieldProcessors, executionContexts, securityOptions);

			return mappingResult.Document;
		}
		/// <summary>
		/// there are some differences in how SOLR stores the standard fields, thus we need to convert a few of the solr fields
		/// into something that synthesis will recognize
		/// </summary>
		/// <param name="document">the Solr results document</param>
		private void ConvertDocumentToSythesisStandardTemplate(Dictionary<string, object> document)
		{
			string textSuffix = (Sitecore.Context.Site == null || Sitecore.Context.Language.Name == Sitecore.Context.Site.Language)? "_t" : "_t_"+Sitecore.Context.Language.Name.ToLower();
			if (document.ContainsKey("__smallcreateddate_tdt"))
				document["__smallcreateddate"] = document["__smallcreateddate_tdt"];
			if (document.ContainsKey("__smallupdateddate_tdt"))
				document["__smallupdateddate"] = document["__smallupdateddate_tdt"];
			if (document.ContainsKey("__display_name" + textSuffix))
				document["__display_name"] = document["__display_name" + textSuffix];
			if (document.ContainsKey("__created_by_s"))
				document["parsedcreatedby"] = document["__created_by_s"];
			//updatedby is not available in the solr index
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
			var arrayList = value as ArrayList;
			if (arrayList == null) return value.ToString();
			// sitecore uses arraylists, and they can't use linq extensions :(
			StringBuilder sb = new StringBuilder();
			foreach (object o in arrayList)
				sb.Append(o).Append("|");
			sb.Remove(sb.Length - 1,1);
			return sb.ToString();
		}

	}
}
