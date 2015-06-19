using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.SolrProvider;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Diagnostics;
using Synthesis.Pipelines;
using Synthesis.Solr.ContentSearch;

namespace Synthesis.Solr.Pipelines.ContentSearch
{
	public class ResolveSolrQueryable
	{
		public void Process(SynthesisSearchContextArgs args)
		{
			Assert.IsNotNull(args, "Args must not be null");
			var solrContext = args.SearchContext as SolrSearchContext;
			if (solrContext != null)
			{
				var overrideMapper = new SynthesisSolrDocumentTypeMapper();
				overrideMapper.Initialize(args.SearchContext.Index);
				var mapperExecutionContext = new OverrideExecutionContext<IIndexDocumentPropertyMapper<Dictionary<string, object>>>(overrideMapper);
				var executionContexts = new List<IExecutionContext>();
				if (args.ExecutionContext != null) executionContexts.AddRange(args.ExecutionContext);
				executionContexts.Add(mapperExecutionContext);
				var m = GetType().GetMethod("GetSolrQueryable", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(args.SynthesisQueryType);
				args.SynthesisQueryable = (IQueryable)m.Invoke(this, new object[] { solrContext, executionContexts.ToArray() });
			}
			else
				throw new NotImplementedException("A Solr index is not being used, if you're using Lucene make sure that you're not overridding the synthesisSearchContext pipeline with the Solr processor");

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
