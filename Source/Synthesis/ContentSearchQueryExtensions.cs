using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace Synthesis
{
	/// <summary>
	/// The ContainsOr is courtesy of Jason Bert (@jason_bert), with minor tweaks.
	/// See: http://www.jasonbert.com/2013/12/18/extending-sitecore-7-search-support-filtering-two-lists/
	/// </summary>
	public static class ContentSearchQueryExtensions
	{
		/// <summary>
		/// Allows you to filter on an enumerable type where you want to match one or more values using logical OR
		/// For example, you may wish to match several template names, or get items using a list of IDs. This is not possible with regular Contains() because that is a logical AND.
		/// </summary>
		public static IQueryable<TSource> ContainsOr<TSource, TKey>(this IQueryable<TSource> queryable, Expression<Func<TSource, TKey>> keySelector, TKey values) 
			where TKey : IEnumerable
		{
			return Contains(queryable, keySelector, values, true);
		}

		/// <summary>
		/// Allows you to filter on an enumerable type where you want to match several values using logical OR on each match value.
		/// For example, you may wish to match several template names, or get items using a list of IDs. (id1 OR id2 OR id3...)
		/// </summary>
		public static IQueryable<TSource> ContainsAnd<TSource, TKey>(this IQueryable<TSource> queryable, Expression<Func<TSource, TKey>> keySelector, TKey values) 
			where TKey : IEnumerable
		{
			return Contains(queryable, keySelector, values, false);
		}

		/// <summary>
		/// Allows you to filter on an enumerable type where you want to match one or more values using logical AND on each match value.
		/// For example: (text1 AND text2 AND text3). This is a more succinct shorthand than using several Contains() clauses.
		/// </summary>
		private static IQueryable<TSource> Contains<TSource, TKey>(IQueryable<TSource> queryable, Expression<Func<TSource, TKey>> keySelector, TKey values, bool or) 
			where TKey : IEnumerable
		{
			// Ensure the body of the selector is a MemberExpression
			if (!(keySelector.Body is MemberExpression))
			{
				throw new InvalidOperationException("Fortis: Expression must be a member expression");
			}

			var typeofTSource = typeof(TSource);
			var typeofTKey = typeof(TKey);

			// x
			var parameter = Expression.Parameter(typeofTSource);

			// Create the enumerable of constant expressions based off of the values
			var constants = values.Cast<object>().Select(Expression.Constant);

			// Create separate MethodCallExpression objects for each constant expression created
			// type -> we need to specify the type which contains the method we want to run
			// methodName -> in this instance we need to specify the Contains method
			// typeArguments -> the type parameter from TKey
			//                  e.g. if we're passing through IEnumerable<Guid> then this will pass through the Guid type
			//                  this is because we're effectively running IEnumerable<Guid>.Contains(Guid guid) for each
			//                  guid in our values object
			// arguments ->
			//      keySelector.Body -> this would be property we want to run the expession on e.g.
			//                          IQueryable<MyPocoTemplate>.Where(x => x.MyIdListField)
			//                          so keySelector.Body will contain the "x.MyIdListField" which is what we want to run
			//                          each constant expression against
			//      constant -> this is the constant expression
			//
			// Each expression will effectively be like running the following;
			// x => x.MyIdListField.Contains(AnId)
			var expressions = constants.Select(constant => Expression.Call(typeof(Enumerable), "Contains", typeofTKey.GetGenericArguments(), keySelector.Body, constant));

			// Combine all the expressions into one expression so you would end with something like;
			// x => x.MyIdListField.Contains(AnId) OR x.MyIdListField.Contains(AnId) OR x.MyIdListField.Contains(AnId)
			var aggregateExpressions = expressions.Select(expression => (Expression)expression).Aggregate((x, y) => or ? Expression.OrElse(x, y) : Expression.AndAlso(x, y));

			// Create the Lambda expression which can be passed to the .Where
			var lambda = Expression.Lambda<Func<TSource, bool>>(aggregateExpressions, parameter);

			return queryable.Where(lambda);
		}

	}
}
