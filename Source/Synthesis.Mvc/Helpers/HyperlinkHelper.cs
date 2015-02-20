using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sitecore.Web.UI.WebControls;
using Synthesis.FieldTypes;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Mvc.Helpers
{
	/// <summary>
	/// HTML helpers to enable simple rendering of Synthesis field types in Sitecore MVC
	/// These are the preferred method of emitting Synthesis models to the markup as they handle
	/// things like HTML encoding and field renderer parameters for you.
	/// 
	/// They all work similar to the form helpers in that they're lambdas on the model:
	/// 
	/// @Html.TextFor(x=>x.MyTextField)
	/// 
	/// You can also use them on non-model Synthesis objects if needed:
	/// 
	/// @Html.ImageFor(x=>someObject.ImageField, "image-class")
	/// </summary>
	public static class HyperlinkHelper
	{
		public static IHtmlString HyperlinkFor<T>(this HtmlHelper<T> helper, Func<T, IHyperlinkField> selector, string linkText = null, string cssClass = null)
		{
			return HyperlinkFor(helper, selector, x =>
			{
				if (linkText != null)
					x.Text = linkText;

				if (cssClass != null)
					x.CssClass = cssClass;
			});
		}

		public static IHtmlString HyperlinkFor<T>(this HtmlHelper<T> helper, Func<T, IHyperlinkField> selector, Action<Link> parameters)
		{
			var field = selector(helper.ViewData.Model);

			if (field.HasValue || Sitecore.Context.PageMode.IsPageEditor)
			{
				var link = new Link();
				link.AttachToLinkField(field);
				parameters(link);

				return new MvcHtmlString(link.RenderAsText());
			}

			return new MvcHtmlString(string.Empty);
		}
	}
}