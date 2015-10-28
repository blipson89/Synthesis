using System.Collections.Concurrent;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.Response.GetModel;
using Sitecore.Mvc.Presentation;
using Synthesis.Mvc.Pipelines.GetRenderer;
using Synthesis.Mvc.Utility;

namespace Synthesis.Mvc.Pipelines.GetModel
{
	
	public class GetFromSynthesis : GetModelProcessor
	{
		private readonly ViewModelTypeResolver _typeResolver;

		public GetFromSynthesis() : this(new ViewModelTypeResolver())
		{
			
		}

		public GetFromSynthesis(ViewModelTypeResolver typeResolver)
		{
			Assert.ArgumentNotNull(typeResolver, "typeResolver");

			_typeResolver = typeResolver;
		}

		protected static ConcurrentDictionary<string, bool> SynthesisRenderingCache = new ConcurrentDictionary<string, bool>(); 

		protected virtual object GetFromViewPath(Rendering rendering, GetModelArgs args)
		{
			var viewPath = rendering.ToString().Replace("View: ", string.Empty);

			if(!SiteHelper.IsValidSite()) return null;

			var useSynthesisModelType = SynthesisRenderingCache.GetOrAdd(rendering.ToString(), key =>
			{
				var renderer = rendering.Renderer;

				var diagnosticRenderer = renderer as RenderingDiagnosticsInjector.DiagnosticsRenderer;
				if (diagnosticRenderer != null) renderer = diagnosticRenderer.InnerRenderer;

				var viewRenderer = renderer as ViewRenderer;
				if (viewRenderer != null) viewPath = viewRenderer.ViewPath;

				var modelType = _typeResolver.GetViewModelType(viewPath);

				// Check to see if no model has been set
				if (modelType == typeof(object)) return false;

				// Check that the model is a Synthesis type (if not, we ignore it)
				if (!typeof(IStandardTemplateItem).IsAssignableFrom(modelType)) return false;

				return true;
			});

			return useSynthesisModelType ? rendering.Item.AsStronglyTyped() : null;
		}

		public override void Process(GetModelArgs args)
		{
			if (args.Result == null)
			{
				args.Result = GetFromViewPath(args.Rendering, args);
			}
		}
	}
}
