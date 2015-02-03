using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Lucene.Net.Documents;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.LuceneProvider;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Exceptions;
using Synthesis.ContentSearch.Lucene;
using Synthesis.Synchronization;

namespace Synthesis
{
	public static class ContentSearchExtensions
	{
		/// <summary>
		/// Gets a queryable for Synthesis items (supports querying on interfaces, unlike the standard method)
		/// </summary>
		/// <typeparam name="TResult">The type of the result item to bind to.</typeparam>
		/// <param name="context">The search context to use</param>
		/// <returns>A queryable item that standard Sitecore LINQ can be used on</returns>
		public static IQueryable<TResult> GetSynthesisQueryable<TResult>(this IProviderSearchContext context)
			where TResult : IStandardTemplateItem
		{
			return GetSynthesisQueryable<TResult>(context, true);
		}

		/// <summary>
		/// Gets a queryable for Synthesis items (supports querying on interfaces, unlike the standard method)
		/// </summary>
		/// <typeparam name="TResult">The type of the result item to bind to.</typeparam>
		/// <param name="context">The search context to use</param>
		/// <param name="applyStandardFilters">Controls whether results will be auto-filtered to context language, correct template, and latest version</param>
		/// <returns>A queryable item that standard Sitecore LINQ can be used on</returns>
		public static IQueryable<TResult> GetSynthesisQueryable<TResult>(this IProviderSearchContext context, bool applyStandardFilters)
			where TResult : IStandardTemplateItem
		{
			return GetSynthesisQueryable<TResult>(context, applyStandardFilters, null);
		}

		/// <summary>
		/// Gets a queryable for Synthesis items
		/// </summary>
		/// <typeparam name="TResult">The type of the result item to bind to.</typeparam>
		/// <param name="context">The search context to use</param>
		/// <param name="applyStandardFilters">Controls whether results will be auto-filtered to context language, correct template, and latest version</param>
		/// <param name="executionContext">The execution context to run the query under</param>
		/// <returns>A queryable item that standard Sitecore LINQ can be used on</returns>
		public static IQueryable<TResult> GetSynthesisQueryable<TResult>(this IProviderSearchContext context, bool applyStandardFilters, params IExecutionContext[] executionContext)
			where TResult : IStandardTemplateItem
		{
			IQueryable<TResult> queryable;

			var luceneContext = context as LuceneSearchContext;

			if (luceneContext != null)
			{
				var overrideMapper = new SynthesisLuceneDocumentTypeMapper();
				overrideMapper.Initialize(context.Index);

				var mapperExecutionContext = new OverrideExecutionContext<IIndexDocumentPropertyMapper<Document>>(overrideMapper);
				var executionContexts = new List<IExecutionContext>();
				if (executionContext != null) executionContexts.AddRange(executionContext);
				executionContexts.Add(mapperExecutionContext);

				queryable = GetLuceneQueryable<TResult>(luceneContext, executionContexts.ToArray());
			}
			else
			{
				// TODO: possible SOLR support with different mapper
				throw new NotImplementedException("At this time Synthesis only supports Lucene indexes.");
			}

			if (applyStandardFilters) queryable = queryable.ApplyStandardFilters();

			return queryable;
		}

		/// <summary>
		/// Applies "standard filters" to a query. Filters by appropriate template, context language, and latest version only.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<TResult> ApplyStandardFilters<TResult>(this IQueryable<TResult> query)
			where TResult : IStandardTemplateItem
		{

			var language = (Sitecore.Context.Item != null) ? Sitecore.Context.Item.Language : Sitecore.Context.Language;

			var subquery = query.Where(x => x.Language == language && x.IsLatestVersion);

			// if the type is IStandardTemplateItem proper, we do not want to filter template ID as that is 'anything'
			if (typeof(TResult) != typeof(IStandardTemplateItem))
			{
				var templateId = GetTemplateId(typeof(TResult));
				return subquery.Where(x => x.TemplateIds.Contains(templateId));
			}

			return subquery;
		}

		/// <summary>
		/// Filters a queryable by all items that have valid database representations. Do not use on monster result sets!
		/// This is useful when you're planning to access database data for all results, and want to filter out any out of date index items
		/// that will throw exceptions when promoted to database later.
		/// 
		/// Note that the queryable will be enumerated and the result of this operation is IEnumerable, not IQueryable.
		/// </summary>
		public static IEnumerable<TResult> WhereResultIsValidDatabaseItem<TResult>(this IQueryable<TResult> queryable)
			where TResult : class, IStandardTemplateItem
		{
			foreach (var result in queryable)
			{
				if (result == null) continue;

				Item currentItem = null;
				try
				{
					currentItem = result.InnerItem;
				}
				catch (InvalidItemException)
				{
				}

				if (currentItem != null) yield return result;
			}
		}

		/// <summary>
		/// Takes the first n valid database-based items in a queryable.
		/// This is useful when you're planning to access database data for all results, and want to filter out any out of date index items
		/// that will throw exceptions when promoted to database later.
		/// 
		/// Note that the queryable will be enumerated until sufficient items are found, and the result of this operation is IEnumerable not IQueryable.
		/// </summary>
		public static IEnumerable<TResult> TakeValidDatabaseItems<TResult>(this IQueryable<TResult> queryable, int numberToTake)
			where TResult : class, IStandardTemplateItem
		{
			int taken = 0;
			foreach (var result in queryable)
			{
				if (result == null) continue;

				Item currentItem = null;
				try
				{
					currentItem = result.InnerItem;
				}
				catch (InvalidItemException)
				{
				}

				if (currentItem != null)
				{
					taken++;
					yield return result;
				}

				if (taken == numberToTake) break;
			}
		}

		private static IQueryable<TResult> GetLuceneQueryable<TResult>(LuceneSearchContext context, IExecutionContext[] executionContext)
			where TResult : IStandardTemplateItem
		{
			var linqToLuceneIndex = new SynthesisLinqToLuceneIndex<TResult>(context, executionContext);

			if (context.Index.Locator.GetInstance<IContentSearchConfigurationSettings>().EnableSearchDebug())
				((IHasTraceWriter)linqToLuceneIndex).TraceWriter = new LoggingTraceWriter(SearchLog.Log);

			return linqToLuceneIndex.GetQueryable();
		}

		private static ID GetTemplateId(Type synthesisType)
		{
			// for concrete types we can use the static ItemTemplateId property thats generated for them
			if (!synthesisType.IsInterface) return GetIdFromConcreteType(synthesisType);

			// for interfaces the template ID is stored in the [RepresentsSitecoreTemplate] attribute
			var representsAttribute = synthesisType.GetCustomAttribute<RepresentsSitecoreTemplateAttribute>();

			if (representsAttribute == null) throw new NotSupportedException("Type " + synthesisType.FullName + " did not have a RepresentsSitecoreTemplateAttribute on it. Try regenerating your model.");

			return ID.Parse(representsAttribute.TemplateId);
		}

		private static ID GetIdFromConcreteType(Type synthesisConcreteType)
		{
			var property = synthesisConcreteType.GetProperty("ItemTemplateId", BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.Public);

			if (property == null) throw new NotSupportedException("Unable to find a template ID for type " + synthesisConcreteType.FullName);

			return (ID)property.GetValue(null);
		}
	}
}
