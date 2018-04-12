using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.LuceneProvider;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Diagnostics;
using Synthesis.Pipelines;

namespace Synthesis.ContentSearch.Lucene
{
	public class ResolveLuceneQueryable : IQueryableResolver
	{
		public IQueryable<TResult> GetSynthesisQueryable<TResult>(SynthesisSearchContextArgs args) where TResult : IStandardTemplateItem
		{
			Assert.IsNotNull(args, "Args must not be null");
			var luceneContext = args.SearchContext as LuceneSearchContext;

			if (luceneContext == null)
				throw new NotImplementedException("A Lucene index is not being used, if you're using Solr make sure that you're overridding the synthesisSearchContext pipeline with the Solr processor in Synthesis.Solr");

			var overrideMapper = new SynthesisLuceneDocumentTypeMapper();
			overrideMapper.Initialize(args.SearchContext.Index);

			var mapperExecutionContext = new OverrideExecutionContext<IIndexDocumentPropertyMapper<Document>>(overrideMapper);
			var executionContexts = new List<IExecutionContext>();
			if (args.ExecutionContext != null) executionContexts.AddRange(args.ExecutionContext);
			executionContexts.Add(mapperExecutionContext);
			return GetLuceneQueryable<TResult>(luceneContext, executionContexts.ToArray());
		}

		private IQueryable<TResult> GetLuceneQueryable<TResult>(LuceneSearchContext context, IExecutionContext[] executionContext)
	where TResult : IStandardTemplateItem
		{
			var linqToLuceneIndex = new SynthesisLinqToLuceneIndex<TResult>(context, executionContext);

			if (context.Index.Locator.GetInstance<IContentSearchConfigurationSettings>().EnableSearchDebug())
				((IHasTraceWriter)linqToLuceneIndex).TraceWriter = new LoggingTraceWriter(SearchLog.Log);

			return linqToLuceneIndex.GetQueryable();
		}
	}
}
