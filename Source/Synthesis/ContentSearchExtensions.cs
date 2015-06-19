using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Exceptions;
using Sitecore.Pipelines;
using Synthesis.Pipelines;
using Synthesis.Synchronization;

namespace Synthesis
{
	public static class ContentSearchExtensions
	{
		/// <summary>
		/// Gets a queryable for Synthesis items (supports querying on interfaces, unlike the standard method)
		/// </summary>
		/// <typeparam name="TResult">The type of the result item to bind to.</typeparam>
		/// <param name="context">The search context to use</param>
		/// <returns>A queryable item that standard Sitecore LINQ can be used on</returns>
		public static IQueryable<TResult> GetSynthesisQueryable<TResult>(this IProviderSearchContext context)
			where TResult : IStandardTemplateItem
		{
			return GetSynthesisQueryable<TResult>(context, true);
		}

		/// <summary>
		/// Gets a queryable for Synthesis items (supports querying on interfaces, unlike the standard method)
		/// </summary>
		/// <typeparam name="TResult">The type of the result item to bind to.</typeparam>
		/// <param name="context">The search context to use</param>
		/// <param name="applyStandardFilters">Controls whether results will be auto-filtered to context language, correct template, and latest version</param>
		/// <returns>A queryable item that standard Sitecore LINQ can be used on</returns>
		public static IQueryable<TResult> GetSynthesisQueryable<TResult>(this IProviderSearchContext context, bool applyStandardFilters)
			where TResult : IStandardTemplateItem
		{
			return GetSynthesisQueryable<TResult>(context, applyStandardFilters, null);
		}

		/// <summary>
		/// Gets a queryable for Synthesis items
		/// </summary>
		/// <typeparam name="TResult">The type of the result item to bind to.</typeparam>
		/// <param name="context">The search context to use</param>
		/// <param name="applyStandardFilters">Controls whether results will be auto-filtered to context language, correct template, and latest version</param>
		/// <param name="executionContext">The execution context to run the query under</param>
		/// <returns>A queryable item that standard Sitecore LINQ can be used on</returns>
		public static IQueryable<TResult> GetSynthesisQueryable<TResult>(this IProviderSearchContext context, bool applyStandardFilters, params IExecutionContext[] executionContext)
			where TResult : IStandardTemplateItem
		{
			var args = new SynthesisSearchContextArgs
			{
				SearchContext = context,
				ExecutionContext = executionContext,
				SynthesisQueryType = typeof (TResult)
			};
			var pipeline = CorePipelineFactory.GetPipeline("synthesisQueryable", string.Empty);
			pipeline.Run(args);
			var result = (IQueryable<TResult>)args.SynthesisQueryable;
			if (applyStandardFilters) result = result.ApplyStandardFilters();
			return result;
		}

		/// <summary>
		/// Applies "standard filters" to a query. Filters by appropriate template, context language, and latest version only.
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="query"></param>
		/// <returns></returns>
		public static IQueryable<TResult> ApplyStandardFilters<TResult>(this IQueryable<TResult> query)
			where TResult : IStandardTemplateItem
		{

			var language = (Context.Item != null) ? Context.Item.Language : Context.Language;

			var subquery = query.Where(x => x.Language == language && x.IsLatestVersion);

			// if the type is IStandardTemplateItem proper, we do not want to filter template ID as that is 'anything'
			if (typeof(TResult) != typeof(IStandardTemplateItem))
			{
				var templateId = GetTemplateId(typeof(TResult));
				return subquery.Where(x => x.TemplateIds.Contains(templateId));
			}

			return subquery;
		}

		/// <summary>
		/// Filters a queryable by all items that have valid database representations. Do not use on monster result sets!
		/// This is useful when you're planning to access database data for all results, and want to filter out any out of date index items
		/// that will throw exceptions when promoted to database later.
		/// 
		/// Note that the queryable will be enumerated and the result of this operation is IEnumerable, not IQueryable.
		/// </summary>
		public static IEnumerable<TResult> WhereResultIsValidDatabaseItem<TResult>(this IQueryable<TResult> queryable)
			where TResult : class, IStandardTemplateItem
		{
			foreach (var result in queryable)
			{
				if (result == null) continue;

				Item currentItem = null;
				try
				{
					currentItem = result.InnerItem;
				}
				catch (InvalidItemException)
				{
				}

				if (currentItem != null) yield return result;
			}
		}

		/// <summary>
		/// Takes the first n valid database-based items in a queryable.
		/// This is useful when you're planning to access database data for all results, and want to filter out any out of date index items
		/// that will throw exceptions when promoted to database later.
		/// 
		/// Note that the queryable will be enumerated until sufficient items are found, and the result of this operation is IEnumerable not IQueryable.
		/// </summary>
		public static IEnumerable<TResult> TakeValidDatabaseItems<TResult>(this IQueryable<TResult> queryable, int numberToTake)
			where TResult : class, IStandardTemplateItem
		{
			int taken = 0;
			foreach (var result in queryable)
			{
				if (result == null) continue;

				Item currentItem = null;
				try
				{
					currentItem = result.InnerItem;
				}
				catch (InvalidItemException)
				{
				}

				if (currentItem != null)
				{
					taken++;
					yield return result;
				}

				if (taken == numberToTake) break;
			}
		}



		private static ID GetTemplateId(Type synthesisType)
		{
			// for concrete types we can use the static ItemTemplateId property thats generated for them
			if (!synthesisType.IsInterface) return GetIdFromConcreteType(synthesisType);

			// for interfaces the template ID is stored in the [RepresentsSitecoreTemplate] attribute
			var representsAttribute = synthesisType.GetCustomAttribute<RepresentsSitecoreTemplateAttribute>();

			if (representsAttribute == null) throw new NotSupportedException("Type " + synthesisType.FullName + " did not have a RepresentsSitecoreTemplateAttribute on it. Try regenerating your model.");

			return ID.Parse(representsAttribute.TemplateId);
		}

		private static ID GetIdFromConcreteType(Type synthesisConcreteType)
		{
			var property = synthesisConcreteType.GetProperty("ItemTemplateId", BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.Public);

			if (property == null) throw new NotSupportedException("Unable to find a template ID for type " + synthesisConcreteType.FullName);

			return (ID)property.GetValue(null);
		}
	}
}
