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
	public static class NumberFields
	{
		public static IHtmlString Render(this IIntegerField field)
		{
			return Render(field, "g");
		}

		public static IHtmlString Render(this IIntegerField field, string format)
		{
			if (Sitecore.Context.PageMode.IsExperienceEditor)
			{
				return new HtmlString(field.RenderedValue);
			}

			if (field.HasValue)
			{
				return new HtmlString(field.Value.ToString(format));
			}

			return new MvcHtmlString(string.Empty);
		}

		public static IHtmlString Render(this INumericField field)
		{
			return Render(field, "g");
		}

		public static IHtmlString Render(this INumericField field, string format)
		{
			if (Sitecore.Context.PageMode.IsExperienceEditor)
			{
				return new HtmlString(field.RenderedValue);
			}

			if (field.HasValue)
			{
				return new HtmlString(field.Value.ToString(format));
			}

			return new MvcHtmlString(string.Empty);
		}
	}
}
