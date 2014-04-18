using System;
using System.Collections;
using System.Collections.Generic;
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
		public static IQueryable<TSource> ContainsOr<TSource, TKey>(this IQueryable<TSource> queryable, Expression<Func<TSource, TKey>> keySelector, IEnumerable values) where TKey : IEnumerable
		{
			return Contains(queryable, keySelector, values, true);
		}

		/// <summary>
		/// Allows you to filter on an enumerable type where you want to match several values using logical OR on each match value.
		/// For example, you may wish to match several template names, or get items using a list of IDs. (id1 OR id2 OR id3...)
		/// </summary>
		public static IQueryable<TSource> ContainsAnd<TSource, TKey>(this IQueryable<TSource> queryable, Expression<Func<TSource, TKey>> keySelector, IEnumerable values) where TKey : IEnumerable
		{
			return Contains(queryable, keySelector, values, false);
		}

		/// <summary>
		/// Allows you to filter on an enumerable type where you want to match one or more values using logical AND on each match value.
		/// For example: (text1 AND text2 AND text3). This is a more succinct shorthand than using several Contains() clauses.
		/// </summary>
		public static IQueryable<TSource> Contains<TSource, TKey>(this IQueryable<TSource> queryable, Expression<Func<TSource, TKey>> keySelector, IEnumerable values, bool orOperator) where TKey : IEnumerable
		{
			const string methodName = "Contains";

			// Ensure the body of the selector is a MemberExpression
			if (!(keySelector.Body is MemberExpression))
			{
				throw new InvalidOperationException("Expression must be a member expression");
			}

			var typeOfTSource = typeof(TSource);
			var typeOfTKey = typeof(TKey);

			// x
			var parameter = Expression.Parameter(typeOfTSource);

			// Create the enumerable of constant expressions based off of the values
			var constants = values.Cast<object>().Select(Expression.Constant);

// ReSharper disable once RedundantAssignment
			IEnumerable<MethodCallExpression> expressions = Enumerable.Empty<MethodCallExpression>();
			/*
			 * Create separate MethodCallExpression objects for each constant expression created
			 *
			 * Each expression will effectively be like running the following;
			 * x => x.MyIdListField.Contains(AnId)
			 *
			 * Check to see if we can find a method on TKey type which matches the method we want to run.
			 * We do this because not all types use the static IEnumerable extension e.g. the String class
			 * has it's own implementation of .Contains.
			 *
			 * If we can't find a matching method then we try to run the extension method found in Enumerable
			 */
			if (typeOfTKey.GetMethods().Any(m => m.Name.Equals(methodName)))
			{
				var method = typeOfTKey.GenericTypeArguments.Any() ? typeOfTKey.GetMethod(methodName, typeOfTKey.GenericTypeArguments) : typeOfTKey.GetMethod(methodName);

				/*
				 * instance		-> this would be property we want to run the expession on e.g.
				 *				   IQueryable<MyPocoTemplate>.Where(x => x.MyIdListField)
				 *				   so keySelector.Body will contain the "x.MyIdListField" which is what we want to run
				 *				   each constant expression against
				 * method		-> the method to run against the instance e.g. "x.MyIdListField.Contains(...)"
				 * arguments	->
				 *		constant	->	this is the constant expression (value) to be passed to the method
				 */
				expressions = constants.Select(constant => Expression.Call(keySelector.Body, method, constant));
			}
			else
			{
				/*
				 * type				->	we need to specify the type which contains the method we want to run
				 * methodName		->	in this instance we need to specify the Contains method
				 * typeArguments	->	the type parameter from TKey
				 * 						e.g. if we're passing through IEnumerable<Guid> then this will pass through the Guid type
				 * 						this is because we're effectively running IEnumerable<Guid>.Contains(Guid guid) for each
				 * 						guid in our values object
				 * arguments		->
				 * 		keySelector.Body	->	this would be property we want to run the expession on e.g.
				 * 								IQueryable<MyPocoTemplate>.Where(x => x.MyIdListField)
				 * 								so keySelector.Body will contain the "x.MyIdListField" which is what we want to run
				 * 								each constant expression against
				 * 		constant			->	this is the constant expression (value) to be passed to the method
				 */
				var typeArgs = typeOfTKey.IsArray ? new[] {typeOfTKey.GetElementType()} : typeOfTKey.GenericTypeArguments;

				expressions = constants.Select(constant => Expression.Call(typeof(Enumerable), methodName, typeArgs, keySelector.Body, constant));
			}

			/* 
			 * Combine all the expressions into one expression so you would end with something like;
			 * 
			 * x => x.MyIdListField.Contains(AnId) OR x.MyIdListField.Contains(AnId) OR x.MyIdListField.Contains(AnId)
			 */
			var aggregateExpressions = expressions.Select(expression => (Expression)expression).Aggregate((x, y) => orOperator ? Expression.OrElse(x, y) : Expression.AndAlso(x, y));

			// Create the Lambda expression which can be passed to the .Where
			var lambda = Expression.Lambda<Func<TSource, bool>>(aggregateExpressions, parameter);

			return queryable.Where(lambda);
		}
	}
}
