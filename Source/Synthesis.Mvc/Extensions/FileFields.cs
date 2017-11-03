using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
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
	public static class FileHelper
	{
		public static IHtmlString Render(this IFileField field, string linkText = null, string cssClass = null)
		{
			return Render(field, linkText, new { @class = cssClass });
		}

		public static IHtmlString Render(this IFileField field, string linkText, object attributes)
		{
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
