using System.Linq;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Lucene;
using Sitecore.ContentSearch.Linq.Parsing;
using Sitecore.ContentSearch.LuceneProvider;

namespace Synthesis.ContentSearch.Hacks
{
	/// <summary>
	/// This class exists to allow us to inject our custom ExpressionParser instance (via the extended GenericQueryable)
	/// </summary>
	public class BugFixIndex<TItem> : LinqToLuceneIndex<TItem>
	{
		public BugFixIndex(LuceneSearchContext context) : this(context, null)
		{
			
		}

		public BugFixIndex(LuceneSearchContext context, IExecutionContext executionContext) : base(context, executionContext)
		{
		}

		public override IQueryable<TItem> GetQueryable()
		{
			GenericQueryable<TItem, LuceneQuery> genericQueryable = new BugFixGenericQueryable<TItem, LuceneQuery>(this, QueryMapper, QueryOptimizer, FieldNameTranslator);
			return genericQueryable;
		}
	}
}