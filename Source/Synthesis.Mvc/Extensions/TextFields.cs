using System.Web;
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
	public static class TextFields
	{
		public static IHtmlString Render(this ITextField field)
		{
			return Render(field, true);
		}

		public static IHtmlString Render(this ITextField field, bool editable, params object[] formatParameters)
		{
			var isEditing = Sitecore.Context.PageMode.IsExperienceEditor;

			if (field.HasTextValue || isEditing)
			{
				if (editable)
				{
					string result = field.RenderedValue;

					if (!isEditing)
					{
						result = Format(field.RenderedValue, formatParameters);
					}

					return new HtmlString(result);
				}

				var richText = field as RichTextField;

				if (richText != null) return new HtmlString(Format(richText.ExpandedLinksValue, formatParameters));

				return new HtmlString(Format(field.RawValue, formatParameters));
			}

			return new HtmlString(string.Empty);
		}

		private static string Format(string value, object[] parameters)
		{
			if (parameters == null || parameters.Length == 0) return value;

			return string.Format(value, parameters);
		}
	}
}
