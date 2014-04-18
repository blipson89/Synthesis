using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lucene.Net.Documents;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Methods;
using Sitecore.ContentSearch.LuceneProvider;
using Sitecore.ContentSearch.Security;
using Sitecore.Data;
using Sitecore.Diagnostics;

namespace Synthesis.ContentSearch
{
	/// <summary>
	/// This mapper extends the default mapper to use Synthesis initializers for mapping types that derive from IStandardTemplateItem, which are much faster than the stock mapper
	/// </summary>
	public class SynthesisDocumentTypeMapper : DefaultLuceneDocumentTypeMapper
	{
		public override TElement MapToType<TElement>(Document document, SelectMethod selectMethod, IEnumerable<IFieldQueryTranslator> virtualFieldProcessors, IEnumerable<IExecutionContext> executionContexts, SearchSecurityOptions securityOptions)
		{
			// if the result type is not IStandardTemplateItem, use the default functionality
			if (!IsSynthesisType<TElement>())
			{
				return base.MapToType<TElement>(document, selectMethod, virtualFieldProcessors, executionContexts, securityOptions);
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

		private static readonly Dictionary<Type, string[]> TypeFieldMapCache = new Dictionary<Type, string[]>();
		private static readonly object TypeFieldMapCacheLock = new object();

		public override IEnumerable<string> GetDocumentFieldsToRead<TElement>(IEnumerable<IExecutionContext> executionContexts)
		{
			if (!IsSynthesisType<TElement>())
			{
				return base.GetDocumentFieldsToRead<TElement>(executionContexts);
			}

			// in Synthesis all the generated classes that have indexable values have [IndexField] explicitly added to them
			// so we can tell which fields we need to grab from the index by using reflection to find all the IndexFieldAttribute
			// that are defined on the target class. The interfaces also have [IndexField] so we're also good there.
			string[] fields;
			Type elementType = typeof (TElement);
			if (!TypeFieldMapCache.TryGetValue(elementType, out fields))
			{
				lock (TypeFieldMapCacheLock)
				{
					if (!TypeFieldMapCache.TryGetValue(elementType, out fields))
					{
						var indexFields = GetPublicProperties(elementType)
							.SelectMany(x => x.GetCustomAttributes(typeof (IndexFieldAttribute), false))
							.Cast<IndexFieldAttribute>()
							.Select(x => x.IndexFieldName)
							.Distinct();

						TypeFieldMapCache[elementType] = indexFields.ToArray();
					}
				}
			}

			return TypeFieldMapCache[elementType];
		}

		private bool IsSynthesisType<TElement>()
		{
			return typeof (IStandardTemplateItem).IsAssignableFrom(typeof (TElement));
		}

		private static PropertyInfo[] GetPublicProperties(Type type)
		{
			if (type.IsInterface)
			{
				var propertyInfos = new List<PropertyInfo>();

				var considered = new List<Type>();
				var queue = new Queue<Type>();
				considered.Add(type);
				queue.Enqueue(type);
				while (queue.Count > 0)
				{
					var subType = queue.Dequeue();
					foreach (var subInterface in subType.GetInterfaces())
					{
						if (considered.Contains(subInterface)) continue;

						considered.Add(subInterface);
						queue.Enqueue(subInterface);
					}

					var typeProperties = subType.GetProperties(
						BindingFlags.FlattenHierarchy
						| BindingFlags.Public
						| BindingFlags.Instance);

					var newPropertyInfos = typeProperties
						.Where(x => !propertyInfos.Contains(x));

					propertyInfos.InsertRange(0, newPropertyInfos);
				}

				return propertyInfos.ToArray();
			}

			return type.GetProperties(BindingFlags.FlattenHierarchy
				| BindingFlags.Public | BindingFlags.Instance);
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