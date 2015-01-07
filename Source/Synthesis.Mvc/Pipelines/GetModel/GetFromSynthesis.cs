using Sitecore.Mvc.Pipelines.Response.GetModel;
using Sitecore.Mvc.Presentation;
using Synthesis.Mvc.Pipelines.GetRenderer;

namespace Synthesis.Mvc.Pipelines.GetModel
{
	
	public class GetFromSynthesis : GetModelProcessor
	{
		protected virtual object GetFromViewPath(Rendering rendering, GetModelArgs args)
		{
			var viewPath = rendering.ToString().Replace("View: ", string.Empty);

			var renderer = rendering.Renderer;

			var diagnosticRenderer = renderer as RenderingDiagnosticsInjector.DiagnosticsRenderer;
			if (diagnosticRenderer != null) renderer = diagnosticRenderer.InnerRenderer;

			var viewRenderer = renderer as ViewRenderer;
			if (viewRenderer != null) viewPath = viewRenderer.ViewPath;

			var modelType = new ViewModelTypeResolver().GetViewModelType(viewPath);

			// Check to see if no model has been set
			if (modelType == typeof(object)) return null;

			// Check that the model is a Synthesis type (if not, we ignore it)
			if (!typeof(IStandardTemplateItem).IsAssignableFrom(modelType)) return null;

			// if we got here we know that the view requested a model of a Synthesis type. We'll give it one.
			// note that we're not validating that the item IS of the correct template, but that's because reflection
			// is slow, and the validation of Synthesis typing here is pretty much to avoid typing models for components
			// that are built into Sitecore.
			return rendering.Item.AsStronglyTyped();
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
