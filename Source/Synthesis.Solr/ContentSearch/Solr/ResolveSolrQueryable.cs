using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.SolrProvider;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Diagnostics;
using Synthesis.ContentSearch;
using Synthesis.Pipelines;

namespace Synthesis.Solr.ContentSearch.Solr
{
	public class ResolveSolrQueryable : IQueryableResolver
	{
		public IQueryable<TResult> GetSynthesisQueryable<TResult>(SynthesisSearchContextArgs args) where TResult : IStandardTemplateItem
		{
			Assert.IsNotNull(args, "Args must not be null");
			var solrContext = args.SearchContext as SolrSearchContext;
			if (solrContext == null)
				throw new NotImplementedException("A Solr index is not being used, if you're using Lucene make sure that you're not overridding the synthesisSearchContext pipeline with the Solr processor");

			var overrideMapper = new SynthesisSolrDocumentTypeMapper();
			overrideMapper.Initialize(args.SearchContext.Index);
			var mapperExecutionContext = new OverrideExecutionContext<IIndexDocumentPropertyMapper<Dictionary<string, object>>>(overrideMapper);
			var executionContexts = new List<IExecutionContext>();
			if (args.ExecutionContext != null) executionContexts.AddRange(args.ExecutionContext);
			executionContexts.Add(mapperExecutionContext);
			return GetSolrQueryable<TResult>(solrContext, executionContexts.ToArray());
		}
		private IQueryable<TResult> GetSolrQueryable<TResult>(SolrSearchContext context, IExecutionContext[] executionContext)
			where TResult : IStandardTemplateItem
		{
			var linqToSolrIndex = new SynthesisLinqToSolrIndex<TResult>(context, executionContext);

			if (context.Index.Locator.GetInstance<IContentSearchConfigurationSettings>().EnableSearchDebug())
				((IHasTraceWriter)linqToSolrIndex).TraceWriter = new LoggingTraceWriter(SearchLog.Log);
			return linqToSolrIndex.GetQueryable();
		}
	}
}
