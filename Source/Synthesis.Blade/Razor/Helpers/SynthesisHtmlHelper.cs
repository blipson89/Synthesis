using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Synthesis.FieldTypes;
using System.Web;
using Sitecore.Web.UI.WebControls;
using Synthesis;
using Synthesis.FieldTypes.Interfaces;

namespace Blade.Razor.Helpers
{
	public static class SynthesisHtmlHelper
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

		public static IHtmlString FileLinkFor<T>(this HtmlHelper<T> helper, Func<T, IFileField> selector)
		{
			return FileLinkFor(helper, selector, null);
		}

		public static IHtmlString FileLinkFor<T>(this HtmlHelper<T> helper, Func<T, IFileField> selector, string linkText)
		{
			return FileLinkFor(helper, selector, linkText, new { });
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
					sb.AppendFormat(" {0}=\"{1}\"", attribute.Name.Replace('_', '-'), HttpUtility.HtmlEncode(attribute.GetValue(attributes, null)));
				}

				sb.Append(">");
				sb.Append(linkText ?? field.Url);
				sb.Append("</a>");

				return new MvcHtmlString(sb.ToString());
			}

			return new MvcHtmlString(string.Empty);
		}

		public static IHtmlString ImageFor<T>(this HtmlHelper<T> helper, Func<T, IImageField> selector)
		{
			return ImageFor(helper, selector, x => { });
		}

		public static IHtmlString ImageFor<T>(this HtmlHelper<T> helper, Func<T, IImageField> selector, string cssClass)
		{
			return ImageFor(helper, selector, x => { x.CssClass = cssClass; });
		}

		public static IHtmlString ImageFor<T>(this HtmlHelper<T> helper, Func<T, IImageField> selector, int maxWidth, int maxHeight)
		{
			return ImageFor(helper, selector, x =>
			{
				x.MaxWidth = maxWidth;
				x.MaxHeight = maxHeight;
			});
		}

		public static IHtmlString ImageFor<T>(this HtmlHelper<T> helper, Func<T, IImageField> selector, int maxWidth, int maxHeight, string cssClass)
		{
			return ImageFor(helper, selector, x =>
			{
				x.MaxWidth = maxWidth;
				x.MaxHeight = maxHeight;
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

		public static IHtmlString HyperlinkFor<T>(this HtmlHelper<T> helper, Func<T, IHyperlinkField> selector)
		{
			return HyperlinkFor(helper, selector, x => { });
		}

		public static IHtmlString HyperlinkFor<T>(this HtmlHelper<T> helper, Func<T, IHyperlinkField> selector, string linkText)
		{
			return HyperlinkFor(helper, selector, x =>
			{
				x.Text = linkText;
			});
		}

		public static IHtmlString HyperlinkFor<T>(this HtmlHelper<T> helper, Func<T, IHyperlinkField> selector, string linkText, string cssClass)
		{
			return HyperlinkFor(helper, selector, x =>
			{
				x.Text = linkText;
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