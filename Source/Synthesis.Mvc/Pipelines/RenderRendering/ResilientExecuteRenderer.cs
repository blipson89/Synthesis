using System;
using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;
using Synthesis.Mvc.Pipelines.GetRenderer;

namespace Synthesis.Mvc.Pipelines.RenderRendering
{
	/* Hat tip: https://github.com/GuitarRich/sitecore-exception-handler */

	public class ResilientExecuteRenderer : ExecuteRenderer
	{
		public string ErrorViewPath { get; set; }

		public string ModelErrorViewPath { get; set; }

		public override void Process(RenderRenderingArgs args)
		{
			try
			{
				base.Process(args);
			}
			catch (Exception ex)
			{
				args.Cacheable = false;

				LogException(ex);

				var processed = ProcessException(ex);

				var model = CreateRenderingErrorModel(args, processed);

				if (DispatchPossibleModelException(model, args.PageContext.RequestContext, args.Writer)) return;

				DispatchException(model, args.PageContext.RequestContext, args.Writer);
			}
		}

		protected virtual void LogException(Exception ex)
		{
			Log.Error("Error occurred rendering a view.", ex, GetType());
		}

		protected virtual Exception ProcessException(Exception exception)
		{
			return exception;
		}

		protected virtual void DispatchException(RenderingErrorModel model, RequestContext context, TextWriter output)
		{
			var viewPath = ResolveErrorViewPath(model, context);

			if (string.IsNullOrWhiteSpace(viewPath))
				throw new Exception("Error occurred while rendering. See the inner exception for details.", model.Exception);
			
			RenderView(viewPath, model, context, output);
		}

		protected virtual bool DispatchPossibleModelException(RenderingErrorModel model, RequestContext context, TextWriter output)
		{
			var invalidOp = model.Exception as InvalidOperationException;

			if (invalidOp?.InnerException != null && invalidOp.InnerException.Message.StartsWith("The model item passed into the dictionary is of type"))
			{
				model.Exception = invalidOp.InnerException;

				if (Context.PageMode.IsPreview || Context.PageMode.IsExperienceEditor)
				{
					var viewPath = ResolveModelErrorViewPath(model, context);

					if (string.IsNullOrWhiteSpace(viewPath))
						CreateViewErrorRenderer().RenderNullModelMessage(new HtmlTextWriter(output), model);
					else RenderView(viewPath, model, context, output);
				}

				return true;
			}

			return false;
		}

		protected virtual void RenderView(string viewName, object model, RequestContext context, TextWriter output)
		{
			Assert.ArgumentNotNullOrEmpty(viewName, "viewName");

			var controllerContext = new ControllerContext(context, new ShimController());

			controllerContext.Controller.ViewData.Model = model;

			var viewEngineResult = ViewEngines.Engines.FindView(controllerContext, viewName, null);

			var viewContext = new ViewContext(
				controllerContext,
				viewEngineResult.View,
				controllerContext.Controller.ViewData,
				controllerContext.Controller.TempData,
				output);

			viewEngineResult.View.Render(viewContext, output);
		}

		protected virtual RenderingErrorModel CreateRenderingErrorModel(RenderRenderingArgs args, Exception ex)
		{
			var renderingPath = CreateRenderingDisplayPathResolver().ResolveRenderingPath(args.Rendering);

			var renderer = args.Rendering.Renderer;

			var diags = renderer as RenderingDiagnosticsInjector.DiagnosticsRenderer;
			if (diags != null) renderer = diags.InnerRenderer;

			var viewRendering = renderer as ViewRenderer;

			var expectedModelType = viewRendering != null ? CreateViewModelTypeResolver().GetViewModelType(renderingPath) : null;

			return new RenderingErrorModel(renderingPath, args.Rendering.DataSource, expectedModelType, args.Rendering.Model, ex);
		}

		protected virtual RenderingDisplayPathResolver CreateRenderingDisplayPathResolver()
		{
			return new RenderingDisplayPathResolver();
		}

		protected virtual ViewModelTypeResolver CreateViewModelTypeResolver()
		{
			return new ViewModelTypeResolver();
		}

		protected virtual ViewErrorRenderer CreateViewErrorRenderer()
		{
			return new ViewErrorRenderer();
		}

		protected virtual string ResolveErrorViewPath(RenderingErrorModel model, RequestContext context)
		{
			return ErrorViewPath;
		}

		protected virtual string ResolveModelErrorViewPath(RenderingErrorModel model, RequestContext context)
		{
			return ModelErrorViewPath;
		}

		protected class ShimController : Controller
		{

		}
	}
}
