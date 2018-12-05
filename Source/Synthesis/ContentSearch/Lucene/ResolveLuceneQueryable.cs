using System;
using System.Linq;
using Synthesis.Pipelines;

namespace Synthesis.ContentSearch.Lucene
{
    [Obsolete("Lucene is no longer supported. All references will be removed in a later version.")]
	public class ResolveLuceneQueryable : IQueryableResolver
	{
	    [Obsolete("Lucene is no longer supported. All references will be removed in a later version.")]
        public IQueryable<TResult> GetSynthesisQueryable<TResult>(SynthesisSearchContextArgs args) where TResult : IStandardTemplateItem
		{
		    throw new NotSupportedException("Lucene is no longer supported with Synthesis. To continue to use Content Search, please install Synthesis.Solr");
		}

	}
}
