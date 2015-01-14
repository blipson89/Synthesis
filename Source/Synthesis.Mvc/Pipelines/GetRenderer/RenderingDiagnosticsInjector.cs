using System;
using System.IO;
using System.Web;
using System.Web.UI;
using Sitecore.Mvc.Pipelines.Response.GetRenderer;
using Sitecore.Mvc.Presentation;
using Synthesis.Mvc.Utility;

namespace Synthesis.Mvc.Pipelines.GetRenderer
{
	public class RenderingDiagnosticsInjector : GetRendererProcessor
	{
		public override void Process(GetRendererArgs args)
		{
			if (args.Result == null) return;

			if(DiagnosticsEnabled)
				args.Result = new DiagnosticsRenderer(args.Result, args.Rendering);
		}

		private static readonly Lazy<bool> DebugEnabled = new Lazy<bool>(() => HttpContext.Current.IsDebuggingEnabled);
		public static bool DiagnosticsEnabledForThisRequest
		{
			get
			{
				var value = HttpContext.Current.Items["RENDERING_DIAGNOSTICS_ENABLED"];
				if (value == null || value.ToString() == bool.TrueString) return true;

				return false;
			}
			set { HttpContext.Current.Items["RENDERING_DIAGNOSTICS_ENABLED"] = value; }
		}

		protected virtual bool DiagnosticsEnabled
		{
			get
			{
				if (!DiagnosticsEnabledForThisRequest) return false;
				if (!DebugEnabled.Value) return false;
				if (!SiteHelper.IsValidSite()) return false;

				return true;
			}
		}

		internal class DiagnosticsRenderer : Renderer
		{
			private readonly Renderer _innerRenderer;
			private readonly Rendering _rendering;

			public DiagnosticsRenderer(Renderer innerRenderer, Rendering rendering)
			{
				_innerRenderer = innerRenderer;
				_rendering = rendering;
			}

			public Renderer InnerRenderer { get { return _innerRenderer; } }

			public override void Render(TextWriter writer)
			{
				string renderingPath = _rendering.RenderingItemPath;

				var viewRendering = _innerRenderer as ViewRenderer;
				if (viewRendering != null) renderingPath = viewRendering.ViewPath;

				var controllerRendering = _innerRenderer as ControllerRenderer;
				if (controllerRendering != null) renderingPath = controllerRendering.ControllerName + "::" + controllerRendering.ActionName;

				using (new RenderingDiagnostics(new HtmlTextWriter(writer), renderingPath, _rendering.Caching))
				{
					_innerRenderer.Render(writer);
				}
			}

			public override string ToString()
			{
				return InnerRenderer != null ? InnerRenderer.ToString() : base.ToString();
			}

			public override string CacheKey
			{
				get { return InnerRenderer != null ? InnerRenderer.CacheKey : base.CacheKey; }
			}
		}

	}
}
