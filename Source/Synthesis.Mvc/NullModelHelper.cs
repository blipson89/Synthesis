using System;
using System.Web.UI;

namespace Synthesis.Mvc
{
	/// <summary>
	/// Global definition of how to render a control when the model is null (across web controls, user controls, and razor controls)
	/// </summary>
	internal static class NullModelHelper
	{
		internal static void RenderNullModelMessage(HtmlTextWriter writer, string viewName, string dataSource, Type modelType, object model)
		{
			writer.AddAttribute("style", "border: 1px dashed red; padding: .5em;");
			writer.RenderBeginTag("div");

			writer.Write("<p><strong>Hidden View:</strong> {0}</p>", viewName);

			writer.Write("<p>View was automatically hidden because the content data source provided did not have the expected data structure.</p>", modelType.Name);

			if (!string.IsNullOrWhiteSpace(dataSource))
				writer.Write("<p>The view's data source value, <span style=\"font-family: monospace\">{0}</span>, may point to an invalid item or an item of the wrong template type. Use the <em>Set Associated Content</em> button to point the data source to a valid item.</p>", dataSource);

			if (model != null && !model.GetType().IsAssignableFrom(modelType))
			{
				writer.Write("<p>The model received was of type <span style=\"font-family: monospace\">{0}</span> which cannot be converted to the expected model type <span style=\"font-family: monospace\">{1}</span></p>", model.GetType().Name, modelType.Name);
			}

			if (model == null)
			{
				writer.Write("<p>The received model was null, and we cannot render the view with a null model (expected type: <span style=\"font-family: monospace\">{0}</span></p>", modelType.Name);
			}

			writer.Write("<p><em>This message is only displayed in preview or edit mode, and will not appear to end users.</em></p>");

			writer.RenderEndTag(); // div
		}
	}
}
