using System.Reflection;
using Lucene.Net.Search;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Lucene;
using Sitecore.ContentSearch.LuceneProvider;

namespace Synthesis.ContentSearch.Lucene
{
	/// <summary>
	/// Plugs Synthesis' field-naming conventions into the standard index
	/// </summary>
	public class SynthesisLinqToLuceneIndex<TItem> : LinqToLuceneIndex<TItem>
	{
		private readonly FieldNameTranslator _fieldNameTranslator;

		public SynthesisLinqToLuceneIndex(LuceneSearchContext context) : this(context, null)
		{
			
		}

		public SynthesisLinqToLuceneIndex(LuceneSearchContext context, params IExecutionContext[] executionContexts)
			: base(context, executionContexts)
		{
			_fieldNameTranslator = new SynthesisFieldNameTranslator(context.Index.FieldNameTranslator);
		}

		protected override FieldNameTranslator FieldNameTranslator
		{
			get { return _fieldNameTranslator; }
		}

		/*
		 * THESE METHODS ARE A TOTAL HACK
		 * They exist to work around a bug in Sitecore (7.x-8.0 at least) where it uses private reflection
		 * in such a way that it breaks all derived classes of LinqToLuceneIndex when GetResults() is called
		 * on the queryable (#4 on GitHub)
		 * 
		 * These basically act as proxies so the private reflection in Sitecore hits these methods,
		 * which then reflect the call down to the base class' private method as is expected.
		 * 
		 * Source of hack: the Execute() method on the base class doing this:
		 * MethodInfo methodInfo1 = this.GetType().GetMethod("ApplySearchMethods", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(type);
		 * MethodInfo methodInfo2 = this.GetType().GetMethod("ApplyScalarMethods", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(typeof (TResult), type);
		 * 
		 * The this.GetType() cause it to look for the private methods on any derived classes, where they do not exist.
		 */
		private object ApplySearchMethods<TElement>(LuceneQuery query, TopDocs searchHits)
		{
			var baseMethod = typeof(LinqToLuceneIndex<TItem>).GetMethod("ApplySearchMethods", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(typeof(TElement));

			return baseMethod.Invoke(this, new object[] { query, searchHits });
		}

		private TResult ApplyScalarMethods<TResult, TDocument>(LuceneQuery query, object processedResults, TopDocs results)
		{
			var baseMethod = typeof(LinqToLuceneIndex<TItem>).GetMethod("ApplyScalarMethods", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(typeof(TResult), typeof(TDocument));

			return (TResult)baseMethod.Invoke(this, new[] { query, processedResults, results });
		}
	}
}
