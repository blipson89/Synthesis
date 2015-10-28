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
	public static class TextHelper
	{
		public static IHtmlString TextFor<T>(this HtmlHelper<T> helper, Func<T, ITextField> selector)
		{
			return TextFor(helper, selector, true);
		}

		public static IHtmlString TextFor<T>(this HtmlHelper<T> helper, Func<T, ITextField> selector, bool editable)
		{
			var field = selector(helper.ViewData.Model);

			if (field.HasTextValue || Sitecore.Context.PageMode.IsPageEditor)
			{
				if (editable)
					return new MvcHtmlString(field.RenderedValue);

				var richText = field as RichTextField;

				if (richText != null) return new MvcHtmlString(richText.ExpandedLinksValue);

				return new MvcHtmlString(field.RawValue);
			}

			return new MvcHtmlString(string.Empty);
		}
	}
}