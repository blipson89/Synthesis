using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sitecore.Resources.Media;
using Sitecore.Web.UI.WebControls;
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
	public static class ImageFields
	{
		public static IHtmlString Render(this IImageField field, string cssClass)
		{
			return Render(field, true, cssClass);
		}
		public static IHtmlString Render(this IImageField field, bool editable)
		{
			return Render(field, editable, "");
		}
		public static IHtmlString Render(this IImageField field, bool editable, string cssClass)
		{
			return Render(field, x =>
			{
				x.CssClass = cssClass;
				x.DisableWebEditing = !editable;
			});
		}

		public static IHtmlString Render(this IImageField field, int? maxWidth = null, int? maxHeight = null, string cssClass = null, bool editable = true)
		{
			return Render(field, x =>
			{
				if (maxWidth.HasValue)
					x.MaxWidth = maxWidth.Value;

				if (maxHeight.HasValue)
					x.MaxHeight = maxHeight.Value;

				if (!editable)
					x.DisableWebEditing = true;

				x.CssClass = cssClass;
			});
		}

		public static IHtmlString Render(this IImageField field, Action<Image> parameters)
		{
			if (field.HasValue || Sitecore.Context.PageMode.IsExperienceEditor)
			{
				var imageRenderer = new Image();
				imageRenderer.AttachToImageField(field);
				parameters(imageRenderer);

				return new MvcHtmlString(imageRenderer.RenderAsText());
			}

			return new MvcHtmlString(string.Empty);
		}

		public static IHtmlString RenderDpiAware(this IImageField field, int? max1XWidth = null, int? max1XHeight = null, string cssClass = null, int maxScale = 2, bool editable = true)
		{
			if (Sitecore.Context.PageMode.IsExperienceEditor || maxScale == 1)
			{
				return Render(field, max1XWidth, max1XHeight, cssClass, editable);
			}

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

					// ReSharper disable once UseStringInterpolation
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
