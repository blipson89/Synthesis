using System;
using System.IO;
using System.Web.UI;
using Sitecore;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;
using Sitecore.Mvc.Presentation;

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
			var baseResult = (ViewRenderer)base.GetRenderer(rendering, args);

			return new ResilientViewRenderer
			{
				ViewPath = baseResult.ViewPath,
				Rendering = baseResult.Rendering
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
							NullModelHelper.RenderNullModelMessage(new HtmlTextWriter(writer), ViewPath, Rendering.DataSource, new ViewModelTypeResolver().GetViewModelType(ViewPath), Rendering.Model);
						}
					}
				}
			}
		}
	}
}
