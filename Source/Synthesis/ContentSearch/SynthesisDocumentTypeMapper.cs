using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Methods;
using Sitecore.Data;
using Synthesis.Configuration;
using Synthesis.Initializers;

namespace Synthesis.ContentSearch
{
	/// <summary>
	/// Generic implementation of mapping indexed items to Synthesis items. This class is used by provider-specific (e.g. Lucene, SOLR) classes to perform common mapping tasks.
	/// </summary>
	public class SynthesisDocumentTypeMapper
	{
		public DocumentMappingResult<TElement> MapToType<TElement>(Func<Dictionary<string, string>> getFieldsMethod, SelectMethod selectMethod)
		{
			// if the result type is not IStandardTemplateItem, use the default functionality
			if (!IsSynthesisType<TElement>())
			{
				return new DocumentMappingResult<TElement>(default(TElement), false);
			}

			// initializers can't really support sub-selects of objects. Error if that's what's being used.
			if (selectMethod != null)
			{
				throw new NotSupportedException("Using Select on a Synthesis object type is supported. Convert the query to a list or array before selecting, then select using LINQ to objects.");
			}

			var evaluatedFields = getFieldsMethod();

			ShortID templateId;
			if (!evaluatedFields.ContainsKey("_template") || !ShortID.TryParse(evaluatedFields["_template"], out templateId))
				templateId = ID.Null.ToShortID();

			var initializer = _overrideInitializer ?? ProviderResolver.FindGlobalInitializer(templateId.ToID());

			var result = initializer.CreateInstanceFromSearch(evaluatedFields);

			if (result is TElement) return new DocumentMappingResult<TElement>((TElement)result, true);

			return new DocumentMappingResult<TElement>(default(TElement), true); // note that this is still 'success', because we mapped onto a Synthesis type so we do not want to use default mapping
		}

		private static readonly Dictionary<Type, string[]> TypeFieldMapCache = new Dictionary<Type, string[]>();
		private static readonly object TypeFieldMapCacheLock = new object();

		public IEnumerable<string> GetDocumentFieldsToRead<TElement>()
		{
			if (!IsSynthesisType<TElement>())
			{
				return null;
			}

			// in Synthesis all the generated classes that have indexable values have [IndexField] explicitly added to them
			// so we can tell which fields we need to grab from the index by using reflection to find all the IndexFieldAttribute
			// that are defined on the target class. The interfaces also have [IndexField] so we're also good there.
			string[] fields;
			Type elementType = typeof(TElement);
			if (!TypeFieldMapCache.TryGetValue(elementType, out fields))
			{
				lock (TypeFieldMapCacheLock)
				{
					if (!TypeFieldMapCache.TryGetValue(elementType, out fields))
					{
						var indexFields = GetPublicProperties(elementType)
							.SelectMany(x => x.GetCustomAttributes(typeof(IndexFieldAttribute), false))
							.Cast<IndexFieldAttribute>()
							.Select(x => x.IndexFieldName)
							.Distinct();

						TypeFieldMapCache[elementType] = indexFields.ToArray();
					}
				}
			}

			return TypeFieldMapCache[elementType];
		}

		private static ITemplateInitializer _overrideInitializer;

		/// <summary>
		/// Forces the Synthesis document mapper to use a specified initializer only.
		/// Useful for testing. Not recommended for production. Not thread safe, do not use in parallel.
		/// </summary>
		public static void OverrideInitializer(ITemplateInitializer forcedInitializer)
		{
			_overrideInitializer = forcedInitializer;
		}

		/// <summary>
		/// Removes any forced initializer that is currently registered.
		/// </summary>
		public static void RemoveOverrideInitializer()
		{
			_overrideInitializer = null;
		}

		private bool IsSynthesisType<TElement>()
		{
			return typeof(IStandardTemplateItem).IsAssignableFrom(typeof(TElement));
		}

		private static IEnumerable<PropertyInfo> GetPublicProperties(Type type)
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
	}
}
