using System;
using System.Linq;
using System.Reflection;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.LuceneProvider;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data;
using Synthesis.ContentSearch.Hacks;
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
		/// Gets a queryable for Synthesis items (supports querying on interfaces, unlike the standard method)
		/// </summary>
		/// <typeparam name="TResult">The type of the result item to bind to.</typeparam>
		/// <param name="context">The search context to use</param>
		/// <param name="applyStandardFilters">Controls whether results will be auto-filtered to context language, correct template, and latest version</param>
		/// <param name="executionContext">The execution context to run the query under</param>
		/// <returns>A queryable item that standard Sitecore LINQ can be used on</returns>
		public static IQueryable<TResult> GetSynthesisQueryable<TResult>(this IProviderSearchContext context, bool applyStandardFilters, IExecutionContext executionContext)
			where TResult : IStandardTemplateItem
		{
			var luceneContext = context as LuceneSearchContext;

			if (luceneContext != null)
			{
				var queryable = GetLuceneQueryable<TResult>(luceneContext, null);
				
				if(applyStandardFilters) queryable = queryable.ApplyStandardFilters();

				return queryable;
			}

			throw new NotSupportedException("At present Synthesis only supports the Lucene provider");
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
			var templateId = GetTemplateId(typeof(TResult));
			var language = (Sitecore.Context.Item != null) ? Sitecore.Context.Item.Language : Sitecore.Context.Language;

			return query.Where(x => x.TemplateIds.Contains(templateId) && x.Language == language && x.IsLatestVersion);
		}

		private static IQueryable<TResult> GetLuceneQueryable<TResult>(LuceneSearchContext context, IExecutionContext executionContext)
			where TResult : IStandardTemplateItem
		{
			// once the hacks in the Hacks namespace are fixed (around update 2, I hear), the commented line below can be used instead of BugFixIndex
			// in fact once Update 3? is released, this class may become largely irrelevant as interface support is coming natively
			//var linqToLuceneIndex = (executionContext == null) ? new LinqToLuceneIndex<TResult>(context) : new LinqToLuceneIndex<TResult>(context, executionContext);
			var linqToLuceneIndex = (executionContext == null)
										? new BugFixIndex<TResult>(context)
										: new BugFixIndex<TResult>(context, executionContext);

			if (ContentSearchConfigurationSettings.EnableSearchDebug)
				((IHasTraceWriter)linqToLuceneIndex).TraceWriter = new LoggingTraceWriter(SearchLog.Log);

			return linqToLuceneIndex.GetQueryable();
		}

		private static ID GetTemplateId(Type synthesisType)
		{
			// for concrete types we can use the static ItemTemplateId property thats generated for them
			if (!synthesisType.IsInterface) return GetIdFromConcreteType(synthesisType);

			// for interfaces the template ID is stored in the [RepresentsSitecoreTemplate] attribute
			var representsAttribute = synthesisType.GetCustomAttribute<RepresentsSitecoreTemplateAttribute>();

			if (representsAttribute == null) throw new NotSupportedException("Type " + synthesisType.FullName + " did not have a RepresentsSitecoreTemplateAttribute on it. Try regenerating your model.");

			return ID.Parse(representsAttribute.TemplateID);
		}

		private static ID GetIdFromConcreteType(Type synthesisConcreteType)
		{
			var property = synthesisConcreteType.GetProperty("ItemTemplateId", BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.Public);

			if (property == null) throw new NotSupportedException("Unable to find a template ID for type " + synthesisConcreteType.FullName);

			return (ID)property.GetValue(null);
		}
	}
}
