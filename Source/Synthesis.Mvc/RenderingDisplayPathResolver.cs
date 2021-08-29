using System;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Presentation;
using Synthesis.Mvc.Pipelines.GetRenderer;

namespace Synthesis.Mvc
{
	public class RenderingDisplayPathResolver
	{
		/// <summary>
		/// Gets the rendering path for the rendering
		/// </summary>
		/// <param name="rendering"></param>
		/// <exception cref="NullReferenceException"></exception>
		/// <returns></returns>
		public virtual string ResolveRenderingPath(Rendering rendering)
		{
			var renderer = rendering.Renderer;

			if (renderer == null)
			{
				Log.Warn($"[{nameof(ResolveRenderingPath)}] No renderer was passed to ResolveRenderingPath", this);
			}

			if (renderer is RenderingDiagnosticsInjector.DiagnosticsRenderer diags) renderer = diags.InnerRenderer;

			switch (renderer)
			{

				case ViewRenderer view:
					return view.ViewPath;
				case ControllerRenderer controller:
					return controller.ControllerName + "::" + controller.ActionName;
				case MethodRenderer method:
					return method.TypeName + "." + method.MethodName + "()";
			}

			if (renderer != null)
			{
				Log.Warn($"[{nameof(ResolveRenderingPath)}] Unhandled renderer passed to {nameof(ResolveRenderingPath)}: {renderer.GetType().FullName}", this);
			}

			if (rendering.Item == null)
			{
				Log.Error($"[{nameof(ResolveRenderingPath)}] Rendering item is null", this);
				return string.Empty;
			}

			return rendering.Item.Paths.FullPath;
		}
	}
}
