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

			if(DiagnosticsEnabled && !args.Rendering.RenderingType.Equals("Layout", StringComparison.OrdinalIgnoreCase))
				args.Result = new DiagnosticsRenderer(args.Result, args.Rendering, CreateRenderingDisplayPathResolver());
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

		public class DiagnosticsRenderer : Renderer
		{
			private readonly Rendering _rendering;
			private readonly RenderingDisplayPathResolver _displayPathResolver;

			public DiagnosticsRenderer(Renderer innerRenderer, Rendering rendering, RenderingDisplayPathResolver displayPathResolver)
			{
				InnerRenderer = innerRenderer;
				_rendering = rendering;
				_displayPathResolver = displayPathResolver;
			}

			public Renderer InnerRenderer { get; private set; }

			public override void Render(TextWriter writer)
			{
				string renderingPath = _displayPathResolver.ResolveRenderingPath(_rendering);

				using (new RenderingDiagnostics(new HtmlTextWriter(writer), renderingPath, _rendering.Caching))
				{
					InnerRenderer.Render(writer);
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

		protected virtual RenderingDisplayPathResolver CreateRenderingDisplayPathResolver()
		{
			return new RenderingDisplayPathResolver();
		}
	}
}
