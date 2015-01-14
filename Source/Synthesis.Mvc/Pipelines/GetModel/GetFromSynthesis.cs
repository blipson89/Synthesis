using System;
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

		protected static ConcurrentDictionary<Guid, bool> SynthesisRenderingCache = new ConcurrentDictionary<Guid, bool>(); 

		protected virtual object GetFromViewPath(Rendering rendering, GetModelArgs args)
		{
			var viewPath = rendering.ToString().Replace("View: ", string.Empty);

			if(!SiteHelper.IsValidSite()) return null;

			// it's from Synthesis and it's in cache
			if (SynthesisRenderingCache.ContainsKey(rendering.UniqueId) && SynthesisRenderingCache[rendering.UniqueId]) return rendering.Item.AsStronglyTyped();

			SynthesisRenderingCache.AddOrUpdate(rendering.UniqueId, true, (key, value) =>
			{
				var renderer = rendering.Renderer;

				var diagnosticRenderer = renderer as RenderingDiagnosticsInjector.DiagnosticsRenderer;
				if (diagnosticRenderer != null) renderer = diagnosticRenderer.InnerRenderer;

				var viewRenderer = renderer as ViewRenderer;
				if (viewRenderer != null) viewPath = viewRenderer.ViewPath;

				var modelType = _typeResolver.GetViewModelType(viewPath);

				// Check to see if no model has been set
				if (modelType == typeof (object)) return false;

				// Check that the model is a Synthesis type (if not, we ignore it)
				if (!typeof (IStandardTemplateItem).IsAssignableFrom(modelType)) return false;

				return true;
			});

			return SynthesisRenderingCache[rendering.UniqueId] ? rendering.Item.AsStronglyTyped() : null;
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
