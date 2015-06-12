using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Exceptions;
using Synthesis.ContentSearch.Lucene;
using Synthesis.Synchronization;
using Sitecore.ContentSearch.SolrProvider;
using Synthesis.Solr.ContentSearch;
using Sitecore.ContentSearch.Linq.Parsing;
using Sitecore.ContentSearch.Linq.Solr;

namespace Synthesis.Solr
{
	public static class ContentSearchExtensions
	{
		/// <summary>
		/// Gets a queryable for Synthesis items (supports querying on interfaces, unlike the standard method)
		/// </summary>
		/// <typeparam name="TResult">The type of the result item to bind to.</typeparam>
		/// <param name="context">The search context to use</param>
		/// <returns>A queryable item that standard Sitecore LINQ can be used on</returns>
		public static IQueryable<TResult> GetSolrSynthesisQueryable<TResult>(this IProviderSearchContext context)
			where TResult : IStandardTemplateItem
		{
			return GetSolrSynthesisQueryable<TResult>(context, true);
		}

		/// <summary>
		/// Gets a queryable for Synthesis items (supports querying on interfaces, unlike the standard method)
		/// </summary>
		/// <typeparam name="TResult">The type of the result item to bind to.</typeparam>
		/// <param name="context">The search context to use</param>
		/// <param name="applyStandardFilters">Controls whether results will be auto-filtered to context language, correct template, and latest version</param>
		/// <returns>A queryable item that standard Sitecore LINQ can be used on</returns>
		public static IQueryable<TResult> GetSolrSynthesisQueryable<TResult>(this IProviderSearchContext context, bool applyStandardFilters)
			where TResult : IStandardTemplateItem
		{
			return GetSolrSynthesisQueryable<TResult>(context, applyStandardFilters, null);
		}
		/// <summary>
		/// Gets a queryable for Synthesis items
		/// </summary>
		/// <typeparam name="TResult">The type of the result item to bind to.</typeparam>
		/// <param name="context">The search context to use</param>
		/// <param name="applyStandardFilters">Controls whether results will be auto-filtered to context language, correct template, and latest version</param>
		/// <param name="executionContext">The execution context to run the query under</param>
		/// <returns>A queryable item that standard Sitecore LINQ can be used on</returns>
		public static IQueryable<TResult> GetSolrSynthesisQueryable<TResult>(this IProviderSearchContext context, bool applyStandardFilters, params IExecutionContext[] executionContext)
			where TResult : IStandardTemplateItem
		{
			IQueryable<TResult> queryable;
			var solrContext = context as SolrSearchContext;
			if (solrContext != null)
			{
				var overrideMapper = new SynthesisSolrDocumentTypeMapper();
				overrideMapper.Initialize(context.Index);
				var mapperExecutionContext = new OverrideExecutionContext<IIndexDocumentPropertyMapper<Dictionary<string,object>>>(overrideMapper);
				var executionContexts = new List<IExecutionContext>();
				if (executionContext != null) executionContexts.AddRange(executionContext);
				executionContexts.Add(mapperExecutionContext);

				queryable = GetSolrQueryable<TResult>(solrContext, executionContexts.ToArray());
			}
			else
				throw new NotImplementedException("This method is only applicable to a SOLR query context.");
			if (applyStandardFilters) queryable = queryable.ApplyStandardFilters();

			return queryable;
		}
		private static IQueryable<TResult> GetSolrQueryable<TResult>(SolrSearchContext context, IExecutionContext[] executionContext)
			where TResult : IStandardTemplateItem
		{
			var linqToSolrIndex = new SynthesisLinqToSolrIndex<TResult>(context, executionContext);
			
			if (context.Index.Locator.GetInstance<IContentSearchConfigurationSettings>().EnableSearchDebug())
				((IHasTraceWriter)linqToSolrIndex).TraceWriter = new LoggingTraceWriter(SearchLog.Log);
			return linqToSolrIndex.GetQueryable();
		}
	}
}
