using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.LuceneProvider;
using Sitecore.Data;
using Sitecore.Diagnostics;

namespace Synthesis.ContentSearch
{
	/// <summary>
	/// This mapper extends the default mapper to use Synthesis initializers for mapping types that derive from IStandardTemplateItem, which are much faster than the stock mapper
	/// </summary>
	public class SynthesisDocumentTypeMapper : DefaultLuceneDocumentTypeMapper
	{
		public override TElement MapToType<TElement>(Document document, Sitecore.ContentSearch.Linq.Methods.SelectMethod selectMethod, IEnumerable<IFieldQueryTranslator> virtualFieldProcessors, Sitecore.ContentSearch.Security.SearchSecurityOptions securityOptions)
		{
			// if the result type is not IStandardTemplateItem, use the default functionality
			if (!typeof(IStandardTemplateItem).IsAssignableFrom(typeof(TElement)))
			{
				return base.MapToType<TElement>(document, selectMethod, virtualFieldProcessors, securityOptions);
			}

			// initializers can't really support sub-selects of objects. Error if that's what's being used.
			if (selectMethod != null)
			{
				throw new NotSupportedException("Using Select on a Synthesis object type is supported. Convert the query to a list or array before selecting, then select using LINQ to objects.");
			}

			var fields = ExtractFieldsFromDocument(document, virtualFieldProcessors);

			ShortID templateId;
			if (!fields.ContainsKey("_template") || !ShortID.TryParse(fields["_template"], out templateId))
				templateId = ID.Null.ToShortID();

			var result = Initializers.GetInitializer(templateId.ToID()).CreateInstanceFromSearch(fields);

			if (result is TElement) return (TElement)result;

			return default(TElement);
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