using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Sitecore.Resources.Media;
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
	public static class ImageHelper
	{
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

		public static IHtmlString DpiAwareImageFor<T>(this HtmlHelper<T> helper, Func<T, IImageField> selector, int? max1XWidth = null, int? max1XHeight = null, string cssClass = null, int maxScale = 2)
		{
			if (Sitecore.Context.PageMode.IsPageEditor || maxScale == 1)
			{
				return ImageFor(helper, selector, max1XWidth, max1XHeight, cssClass);
			}

			var field = selector(helper.ViewData.Model);

			if (field.HasValue)
			{
				string mediaUrl = MediaManager.GetMediaUrl(field.MediaItem);
				
				var html = new StringBuilder();
				html.AppendFormat("<img src=\"{0}\"", GenerateImageUrl(mediaUrl, max1XWidth, max1XHeight));		

				var srcset = new List<string>();
				for (int scaleFactor = 2; scaleFactor <= maxScale; scaleFactor++)
				{
					int scaledMaxWidth = (max1XWidth ?? 0) * scaleFactor;
					int scaledMaxHeight = (max1XHeight ?? 0) * scaleFactor;

					srcset.Add(string.Format("{0} {1}x", HttpUtility.UrlPathEncode(GenerateImageUrl(mediaUrl, scaledMaxWidth, scaledMaxHeight)), scaleFactor));
				}

				html.AppendFormat(" srcset=\"{0}\"", string.Join(", ", srcset));

				if (!string.IsNullOrEmpty(field.MediaItem.Alt))
				{
					html.AppendFormat(" alt=\"{0}\"", field.MediaItem.Alt);
				}

				if (!string.IsNullOrEmpty(cssClass))
				{
					html.AppendFormat(" class=\"{0}\"", cssClass);
				}

				html.Append(">");

				return new MvcHtmlString(html.ToString());
			}

			return new MvcHtmlString(string.Empty);
		}

		private static string GenerateImageUrl(string baseUrl, int? maxWidth, int? maxHeight)
		{
			var url = new StringBuilder();

			url.Append(baseUrl);
			if (maxWidth > 0) url.AppendFormat("?mw={0}", maxWidth);
			if (maxHeight > 0) url.AppendFormat("{0}mh={1}", maxWidth > 0 ? "&" : "?", maxHeight);

			return HashingUtils.ProtectAssetUrl(url.ToString());
		}
	}
}