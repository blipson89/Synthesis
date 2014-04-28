using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Methods;
using Sitecore.ContentSearch.LuceneProvider;
using Sitecore.ContentSearch.Security;
using Sitecore.Diagnostics;

namespace Synthesis.ContentSearch.Lucene
{
	/// <summary>
	/// This mapper extends the default mapper to use Synthesis initializers for mapping types that derive from IStandardTemplateItem, which are much faster than the stock mapper
	/// </summary>
	public class SynthesisLuceneDocumentTypeMapper : DefaultLuceneDocumentTypeMapper
	{
		private readonly SynthesisDocumentTypeMapper _synthesisMapper = new SynthesisDocumentTypeMapper();

		public override TElement MapToType<TElement>(Document document, SelectMethod selectMethod, IEnumerable<IFieldQueryTranslator> virtualFieldProcessors, IEnumerable<IExecutionContext> executionContexts, SearchSecurityOptions securityOptions)
		{
			var mappingResult = _synthesisMapper.MapToType<TElement>(() => ExtractFieldsFromDocument(document, virtualFieldProcessors), selectMethod);

			// if the result type is not IStandardTemplateItem, use the default functionality
			if(!mappingResult.MappedSuccessfully)
				return base.MapToType<TElement>(document, selectMethod, virtualFieldProcessors, executionContexts, securityOptions);

			return mappingResult.Document;
		}

		public override IEnumerable<string> GetDocumentFieldsToRead<TElement>(IEnumerable<IExecutionContext> executionContexts)
		{
			var fields = _synthesisMapper.GetDocumentFieldsToRead<TElement>();

			return fields ?? base.GetDocumentFieldsToRead<TElement>(executionContexts);
		}

		private Dictionary<string, string> ExtractFieldsFromDocument(Document document, IEnumerable<IFieldQueryTranslator> virtualFieldProcessors)
		{
			Assert.ArgumentNotNull(document, "document");

			IDictionary<string, object> dictionary = new Dictionary<string, object>();

			foreach (var grouping in document.GetFields().GroupBy(f => f.Name))
			{
				if (grouping.Count() > 1)
				{
					dictionary[grouping.Key] = string.Join("|", grouping.Select(x => x.StringValue));
				}
				else
					dictionary[grouping.Key] = grouping.First().StringValue;
			}

			if (virtualFieldProcessors != null)
				dictionary = virtualFieldProcessors.Aggregate(dictionary, (current, processor) => processor.TranslateFieldResult(current, index.FieldNameTranslator));

			return dictionary.ToDictionary(x => x.Key, x => x.Value.ToString());
		}
	}
}