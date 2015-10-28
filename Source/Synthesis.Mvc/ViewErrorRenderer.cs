using System.IO;
using System.Web.UI;
using Synthesis.Utility;

namespace Synthesis.Mvc
{
	/// <summary>
	/// Global definition of how to render a control when the model is null (across web controls, user controls, and razor controls)
	/// </summary>
	public class ViewErrorRenderer
	{
		public virtual void RenderNullModelMessage(HtmlTextWriter writer, RenderingErrorModel model)
		{
			writer.AddAttribute("style", "border: 1px dashed red; padding: .5em;");
			writer.RenderBeginTag("div");

			writer.Write("<p><strong>Hidden Rendering:</strong> {0}</p>", model.ViewName);

			writer.Write("<p>Rendering was hidden because the content data source provided did not have the expected data structure.</p>");

			if (!string.IsNullOrWhiteSpace(model.DataSource))
				writer.Write("<p>The rendering's data source value, <span style=\"font-family: monospace\">{0}</span>, may point to an invalid Sitecore item or an item of the wrong template type. Use the <em>Set Associated Content</em> button to point the data source to a valid item.</p>", model.DataSource);

			if (model.Model != null && model.ModelType != null && !model.ModelType.IsInstanceOfType(model.Model))
			{
				writer.Write("<p>The model received was of type <span style=\"font-family: monospace\">{0}</span> which cannot be converted to the expected model type <span style=\"font-family: monospace\">{1}</span></p>", model.Model.GetType().Name, model.ModelType.Name);
			}
			else if (model.Model != null && model.ModelType != null)
			{
				writer.Write("<p>The model for <span style=\"font-family: monospace\">{0}</span> was a <strong>valid</strong> model type. <strong>This means a partial view included on the current rendering had an invalid model type.</strong> Unfortunately, we can't be sure which partial caused the problem.</p>", Path.GetFileName(model.ViewName));
			}

			if (model.Model == null)
			{
				writer.Write("<p>The received model was null, and we cannot render the view with a null model (expected type: <span style=\"font-family: monospace\">{0}</span></p>", model.ModelType != null ? model.ModelType.Name : "unknown");
			}

			if (DebugUtility.IsDynamicDebugEnabled)
			{
				writer.Write("<p><span style=\"font-family: monospace; font-weight: bold;\">{0}</span></p><pre>{1}</pre>", model.Exception.Message, model.Exception.StackTrace);
			}

			writer.Write("<p><em>This message is only displayed in preview or edit mode, and will not appear to end users.</em></p>");

			writer.RenderEndTag(); // div
		}
	}
}
