using System;
using System.Web;
using System.Web.Mvc;
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
	public static class DateFields
	{
		public static IHtmlString Render(this IDateTimeField field)
		{
			return Render(field, "g", true);
		}

		public static IHtmlString Render(this IDateTimeField field, bool editable)
		{
			return Render(field, "g", editable);
		}

		public static IHtmlString Render(this IDateTimeField field, string format)
		{
			return Render(field, format, true);
		}

		public static IHtmlString Render(this IDateTimeField field, string format, bool editable)
		{
			return Render(field, x =>
			{
				x.Format = format;
				x.DisableWebEditing = !editable;
			});
		}

		public static IHtmlString Render(IDateTimeField field, Action<Date> parameters)
		{
			if (field.HasValue || Sitecore.Context.PageMode.IsExperienceEditor)
			{
				var date = new Date();
				date.AttachToDateTimeField(field);
				parameters(date);

				return new MvcHtmlString(date.RenderAsText());
			}

			return new MvcHtmlString(string.Empty);
		}
	}
}
