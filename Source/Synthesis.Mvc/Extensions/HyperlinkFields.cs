using System;
using System.Web;
using System.Web.Mvc;
using Sitecore.Web.UI.WebControls;
using Synthesis.FieldTypes;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Mvc.Extensions
{
	/// <summary>
	/// Extensions to enable simple rendering of Synthesis field types in Sitecore MVC
	/// These are the preferred method of emitting Synthesis models to the markup as they handle
	/// things like HTML encoding and field renderer parameters for you.
	///
	/// They are very simple to use:
	///
	/// @mySynthesisObject.Field.Render()
	/// </summary>
	public static class HyperlinkFields
	{
		public static IHtmlString Render(this IHyperlinkField field, string linkText = null, string cssClass = null)
		{
			return Render(field, x =>
			{
				if (linkText != null)
					x.Text = linkText;

				if (cssClass != null)
					x.CssClass = cssClass;
			});
		}

		public static IHtmlString Render(this IHyperlinkField field, Action<Link> parameters)
		{
			if (field.HasValue || Sitecore.Context.PageMode.IsExperienceEditor)
			{
				var link = new Link();
				link.AttachToLinkField(field);
				parameters(link);

				return new MvcHtmlString(link.RenderAsText());
			}

			return new MvcHtmlString(string.Empty);
		}

		public static IDisposable RenderWithBody<T>(this IHyperlinkField field, HtmlHelper<T> helper, string cssClass = null)
		{
			object parameters = new { haschildren = true };

			if (cssClass != null)
			{
				parameters = new { haschildren = true, @class = cssClass };
			}

			return new TagRenderingContext<T>(helper, (FieldType)field, parameters);
		}
	}
}
