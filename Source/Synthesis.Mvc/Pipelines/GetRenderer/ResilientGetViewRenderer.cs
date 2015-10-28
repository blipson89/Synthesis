using System;
using System.IO;
using System.Web.UI;
using Sitecore;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;
using Sitecore.Mvc.Presentation;
using Synthesis.Mvc.Utility;

namespace Synthesis.Mvc.Pipelines.GetRenderer
{
	/// <summary>
	/// Errors in model type no longer explode the whole page, instead showing a warning to preview/editors
	/// 
	/// This prevents view renderings with invalid or lacking data sources from causing major problems.
	/// </summary>
	public class ResilientGetViewRenderer : GetViewRenderer
	{
		protected override Renderer GetRenderer(Rendering rendering, GetRendererArgs args)
		{
			var baseResult = base.GetRenderer(rendering, args);

			if (!SiteHelper.IsValidSite()) return baseResult;

			var viewRenderer = baseResult as ViewRenderer;

			if (viewRenderer == null) return baseResult;

			return new ResilientViewRenderer
			{
				ViewPath = viewRenderer.ViewPath,
				Rendering = viewRenderer.Rendering
			};
		}

		protected class ResilientViewRenderer : ViewRenderer
		{
			// prevent exceptions in rendering caused by invalid model types from exploding the whole page
			public override void Render(TextWriter writer)
			{
				try
				{
					base.Render(writer);
				}
				catch (InvalidOperationException ioe)
				{
					if (ioe.InnerException != null && ioe.InnerException.Message.StartsWith("The model item passed into the dictionary is of type"))
					{
						if (Context.PageMode.IsPreview || Context.PageMode.IsPageEditor)
						{
							var absolutePath = GetAbsoluteViewPath();
							NullModelHelper.RenderNullModelMessage(new HtmlTextWriter(writer), absolutePath, Rendering.DataSource, new ViewModelTypeResolver().GetViewModelType(absolutePath), Rendering.Model, ioe.InnerException);
						}
						
						return;
					}

					throw;
				}
			}
		}
	}
}
