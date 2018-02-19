using System.Web.Mvc;
using Sitecore.ContentSearch;
using Sitecore.ExperienceExplorer.Core.State;
using Sitecore.Mvc.Presentation;
using Sitecore.Sites;
using Synthesis.FieldTypes.Adapters;

namespace Synthesis.Mvc.UI
{
	public class SitecoreRenderingContext : IRenderingContext
	{
		public TItem GetRenderingDatasource<TItem>() where TItem : class, IStandardTemplateItem
		{
			return RenderingContext.CurrentOrNull?.Rendering?.Item.As<TItem>();
		}

		public IStandardTemplateItem RenderingDatasource => GetRenderingDatasource<IStandardTemplateItem>();

		public TItem GetContextItem<TItem>() where TItem : class, IStandardTemplateItem
		{
			return Sitecore.Context.Item.As<TItem>();
		}

		public IStandardTemplateItem ContextItem => GetContextItem<IStandardTemplateItem>();

		public bool IsEditing => Sitecore.Context.Site.DisplayMode == DisplayMode.Edit;
		public bool IsPreview => Sitecore.Context.Site.DisplayMode == DisplayMode.Preview;
		public bool IsNormal => Sitecore.Context.Site.DisplayMode == DisplayMode.Normal;
		public bool IsExperienceExplorer => DependencyResolver.Current.GetService<IExplorerContext>()?.IsExplorerMode() ?? false;
		public bool IsDebugging => Sitecore.Context.PageMode.IsDebugging || Sitecore.Context.Diagnostics.Profiling || Sitecore.Context.Diagnostics.Tracing;
		public bool IsCompletelyNormal => IsNormal && !IsExperienceExplorer && !IsDebugging;

		public SiteContext ContextSite => Sitecore.Context.Site;

		public IDatabaseAdapter ContextDatabase => new DatabaseAdapter(Sitecore.Context.ContentDatabase ?? Sitecore.Context.Database);

		public RenderingParameters Parameters => RenderingContext.CurrentOrNull?.Rendering?.Parameters ?? new RenderingParameters(string.Empty);

		public ISearchIndex ContextIndex
		{
			get
			{
				var contextItem = ContextItem?.InnerItem;

				if (contextItem == null) return null;

				return ContentSearchManager.GetIndex(new SitecoreIndexableItem(contextItem));
			}
		}
	}
}
