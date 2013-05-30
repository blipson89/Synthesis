using System;
using System.Linq;
using System.Linq.Expressions;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.Linq.Indexing;
using Sitecore.ContentSearch.Linq.Parsing;

namespace Synthesis.ContentSearch.Hacks
{
	/// <summary>
	/// This class exists to allow us to inject our custom ExpressionParser instance (via the extended GenericQueryable)
	/// </summary>
	public class BugFixGenericQueryable<TElement, TQuery> : GenericQueryable<TElement, TQuery>
	{
		public BugFixGenericQueryable(Index<TElement, TQuery> index, QueryMapper<TQuery> queryMapper, IQueryOptimizer queryOptimizer, FieldNameTranslator fieldNameTranslator) : base(index, queryMapper, queryOptimizer, fieldNameTranslator)
		{
		}

		protected BugFixGenericQueryable(Index<TQuery> index, QueryMapper<TQuery> queryMapper, IQueryOptimizer queryOptimizer, Expression expression, Type itemType, FieldNameTranslator fieldNameTranslator) : base(index, queryMapper, queryOptimizer, expression, itemType, fieldNameTranslator)
		{
		}

		protected override TQuery GetQuery(Expression expression)
		{
			Trace(expression, "Expression");

			// here we inject a special expression parser that fixes a few issues slated to be resolved in later releases of SC7
			IndexQuery rawQuery = new BugFixExpressionParser(typeof(TElement), ItemType, FieldNameTranslator).Parse(expression);

			Trace(rawQuery, "Raw query:");
			IndexQuery optimizedQuery = QueryOptimizer.Optimize(rawQuery);
			Trace(optimizedQuery, "Optimized query:");
			TQuery mappedQuery = QueryMapper.MapQuery(optimizedQuery);
	
			return mappedQuery;
		}

		public override IQueryable<TQueryElement> CreateQuery<TQueryElement>(Expression expression)
		{
			// use the special generic queryable instance that injects the custom expression parser
			return new BugFixGenericQueryable<TQueryElement, TQuery>(Index, QueryMapper, QueryOptimizer, expression, ItemType, FieldNameTranslator);
		}
	}
}