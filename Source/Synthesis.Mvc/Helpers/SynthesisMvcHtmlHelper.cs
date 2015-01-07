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
	public static class SynthesisMvcHtmlHelper
	{
		public static IHtmlString DateTimeFor<T>(this HtmlHelper<T> helper, Func<T, IDateTimeField> selector)
		{
			return DateTimeFor(helper, selector, "g");
		}

		public static IHtmlString DateTimeFor<T>(this HtmlHelper<T> helper, Func<T, IDateTimeField> selector, string format)
		{
			return DateTimeFor(helper, selector, x => { x.Format = format; });
		}

		public static IHtmlString DateTimeFor<T>(this HtmlHelper<T> helper, Func<T, IDateTimeField> selector, Action<Date> parameters)
		{
			var field = selector(helper.ViewData.Model);

			if (field.HasValue || Sitecore.Context.PageMode.IsPageEditor)
			{
				var date = new Date();
				date.AttachToDateTimeField(field);
				parameters(date);

				return new MvcHtmlString(date.RenderAsText());
			}

			return new MvcHtmlString(string.Empty);
		}

		public static IHtmlString FileLinkFor<T>(this HtmlHelper<T> helper, Func<T, IFileField> selector, string linkText = null, string cssClass = null)
		{
			return FileLinkFor(helper, selector, linkText, new { @class = cssClass });
		}

		public static IHtmlString FileLinkFor<T>(this HtmlHelper<T> helper, Func<T, IFileField> selector, string linkText, object attributes)
		{
			var field = selector(helper.ViewData.Model);

			if (field.HasValue)
			{
				var sb = new StringBuilder();
				sb.Append("<a href=\"");
				sb.Append(HttpUtility.HtmlEncode(field.Url));
				sb.Append("\"");

				foreach (var attribute in attributes.GetType().GetProperties().Where(x => x.CanRead))
				{
					var value = attribute.GetValue(attributes, null);

					if (value == null) continue;

					sb.AppendFormat(" {0}=\"{1}\"", attribute.Name.Replace('_', '-'), HttpUtility.HtmlEncode(value));
				}

				sb.Append(">");
				sb.Append(linkText ?? field.Url);
				sb.Append("</a>");

				return new MvcHtmlString(sb.ToString());
			}

			return new MvcHtmlString(string.Empty);
		}

		public static IHtmlString ImageFor<T>(this HtmlHelper<T> helper, Func<T, IImageField> selector, string cssClass)
		{
			return ImageFor(helper, selector, x => { x.CssClass = cssClass; });
		}

		public static IHtmlString ImageFor<T>(this HtmlHelper<T> helper, Func<T, IImageField> selector, int? maxWidth = null, int? maxHeight = null, string cssClass = null)
		{
			return ImageFor(helper, selector, x =>
			{
				if (maxWidth.HasValue)
					x.MaxWidth = maxWidth.Value;

				if (maxHeight.HasValue)
					x.MaxHeight = maxHeight.Value;

				x.CssClass = cssClass;
			});
		}

		public static IHtmlString ImageFor<T>(this HtmlHelper<T> helper, Func<T, IImageField> selector, Action<Image> parameters)
		{
			var field = selector(helper.ViewData.Model);

			if (field.HasValue || Sitecore.Context.PageMode.IsPageEditor)
			{
				var imageRenderer = new Image();
				imageRenderer.AttachToImageField(field);
				parameters(imageRenderer);

				return new MvcHtmlString(imageRenderer.RenderAsText());
			}

			return new MvcHtmlString(string.Empty);
		}

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