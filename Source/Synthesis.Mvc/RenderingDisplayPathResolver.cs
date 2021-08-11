using System;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Presentation;
using Synthesis.Mvc.Pipelines.GetRenderer;

namespace Synthesis.Mvc
{
	public class RenderingDisplayPathResolver
	{
		private const string SUPPRESS_NULL_ITEM_SETTING_NAME = "Synthesis.RenderingDisplayPathResolver.SuppressNullItemException";
		private readonly bool _suppressNullItemException = Settings.GetBoolSetting(SUPPRESS_NULL_ITEM_SETTING_NAME, false);
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
				if (_suppressNullItemException)
				{
					Log.Error($"[{nameof(ResolveRenderingPath)}] Rendering item is null", this);
					return string.Empty;
				}

				throw new NullReferenceException($"[{nameof(ResolveRenderingPath)}] Rendering item is null");
			}

			return rendering.Item.Paths.FullPath;
		}
	}
}
