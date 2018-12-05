using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.SolrProvider;
using Sitecore.Diagnostics;
using Synthesis.ContentSearch;
using Synthesis.Pipelines;

namespace Synthesis.Solr.ContentSearch
{
	public class ResolveSolrQueryable : IQueryableResolver
	{
	    private readonly IFieldNameTranslatorFactory _fieldNameTranslator;

	    public ResolveSolrQueryable(IFieldNameTranslatorFactory fieldNameTranslator)
	    {
	        _fieldNameTranslator = fieldNameTranslator;
	    }
		public IQueryable<TResult> GetSynthesisQueryable<TResult>(SynthesisSearchContextArgs args) where TResult : IStandardTemplateItem
		{
			Assert.IsNotNull(args, "Args must not be null");
		    if (!(args.SearchContext is SolrSearchContext solrContext))
				throw new NotImplementedException("A Solr index is not being used, if you're using Azure make sure that you're not overridding the synthesisSearchContext pipeline with the Solr processor");

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
		    context.Index.FieldNameTranslator = _fieldNameTranslator.GetFieldNameTranslator(context.Index);
            return context.GetQueryable<TResult>(executionContext);
		}
	}
}
