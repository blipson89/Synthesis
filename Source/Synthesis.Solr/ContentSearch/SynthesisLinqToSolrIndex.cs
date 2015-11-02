using System.Collections.Generic;
using System.Reflection;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Solr;
using Sitecore.ContentSearch.SolrProvider;
using SolrNet;

namespace Synthesis.Solr.ContentSearch
{
	public class SynthesisLinqToSolrIndex<T> : LinqToSolrIndex<T>
	{
		private readonly FieldNameTranslator _fieldNameTranslator;
		public SynthesisLinqToSolrIndex(SolrSearchContext context) : this(context, null)
		{
		}
		public SynthesisLinqToSolrIndex(SolrSearchContext context, params IExecutionContext[] executionContexts)
			: base(context, executionContexts)
		{
			_fieldNameTranslator = new SynthesisSolrFieldNameTranslator(context, context.Index.FieldNameTranslator); ;
		}
		protected override FieldNameTranslator FieldNameTranslator
		{
			get { return _fieldNameTranslator; }
		}
		/*
		 * THIS METHOD IS A TOTAL HACK
		 * They exist to work around a bug in Sitecore (7.x-8.0 at least) where it uses private reflection
		 * in such a way that it breaks all derived classes of LinqToSolrIndex when the solr query is mapped
		 * in a particular way, such as when using a database name in your search query.
		 * 
		 * These basically act as proxies so the private reflection in Sitecore hits these methods,
		 * which then reflect the call down to the base class' private method as is expected.
		 * 
		 * Source of hack: the Execute() method on the base class doing this:
		 * MethodInfo methodInfo = this.GetType().GetMethod("ApplyScalarMethods", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(typeof (TResult), resultType);
		 * 
		 * The this.GetType() cause it to look for the private methods on any derived classes, where they do not exist.
		 */
		private TResult ApplyScalarMethods<TResult, TDocument>(SolrCompositeQuery compositeQuery, object processedResults, SolrQueryResults<Dictionary<string, object>> results)
		{
			var baseMethod = typeof(LinqToSolrIndex<T>).GetMethod("ApplyScalarMethods", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(typeof(TResult), typeof(TDocument));

			return (TResult)baseMethod.Invoke(this, new[] { compositeQuery, processedResults, results });
		}
	}
}
