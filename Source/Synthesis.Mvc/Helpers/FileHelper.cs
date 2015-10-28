using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sitecore;
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
	public static class FileHelper
	{
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
	}
}