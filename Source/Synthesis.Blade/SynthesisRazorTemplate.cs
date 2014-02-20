using System.Web.UI;
using Blade.Views;

namespace Synthesis.Blade
{
	/// <summary>
	/// Version of the static Razor template web control that automatically snags the Synthesis context item as a model
	/// </summary>
	public class SynthesisRazorTemplate : RazorTemplate
	{
		public SynthesisRazorTemplate()
		{
			Model = Sitecore.Context.Item.AsStronglyTyped();
		}
		protected override void DoRender(HtmlTextWriter output)
		{
			if (Model != null)
				base.DoRender(output);
		}
	}
}
