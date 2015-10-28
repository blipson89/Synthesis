using Sitecore.Mvc.Presentation;
using Synthesis.Mvc.Pipelines.GetRenderer;

namespace Synthesis.Mvc
{
	public class RenderingDisplayPathResolver
	{
		public virtual string ResolveRenderingPath(Rendering rendering)
		{
			var renderer = rendering.Renderer;

			var diags = renderer as RenderingDiagnosticsInjector.DiagnosticsRenderer;
			if (diags != null) renderer = diags.InnerRenderer;

			var view = renderer as ViewRenderer;
			if (view != null) return view.ViewPath;

			var controller = renderer as ControllerRenderer;
			if (controller != null) return controller.ControllerName + "::" + controller.ActionName;

			var method = renderer as MethodRenderer;
			if (method != null) return method.TypeName + "." + method.MethodName + "()";

			return rendering.Item.Paths.FullPath;
		}
	}
}
