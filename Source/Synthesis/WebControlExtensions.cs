namespace Synthesis
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Sitecore.Web.UI.WebControls;
using Synthesis.FieldTypes;
	using System.Web.UI.WebControls;
	using System.Diagnostics.CodeAnalysis;

	public static class WebControlExtensions
	{
		/// <summary>
		/// Attaches a Synthesis date field to a Sitecore Date web control
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification="Doesn't make semantic sense")]
		public static void AttachToDateTimeField(this Date date, DateTimeField field)
		{
			date.Item = field.InnerField.Item;
			date.Field = field.Id.ToString();
		}

		/// <summary>
		/// Attaches the HyperLink web control to a FileField as a download link. The control is set to not visible if the media item does not have a value and we aren't page editing. 
		/// </summary>
		public static void AttachToFileField(this HyperLink link, FileField field)
		{
			if (!field.HasValue && !Sitecore.Context.PageMode.IsPageEditor) 
			{ 
				link.Visible = false; 
				return; 
			}

			link.NavigateUrl = field.Url;
		}

		/// <summary>
		/// Attaches a Sitecore Link control to a Synthesis HyperlinkField
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Doesn't make semantic sense")]
		public static void AttachToLinkField(this Link link, HyperlinkField field)
		{
			link.Item = field.InnerField.Item;
			link.Field = field.InnerField.ID.ToString();
		}

		/// <summary>
		/// Attaches the image to a Synthesis ImageField. The control is set to not visible if the media item does not have a value and we aren't page editing.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Doesn't make semantic sense")]
		public static void AttachToImageField(this Sitecore.Web.UI.WebControls.Image image, Synthesis.FieldTypes.ImageField field)
		{
			if (!field.HasValue && !Sitecore.Context.PageMode.IsPageEditor)
			{
				image.Visible = false;
				return;
			}

			image.Item = field.InnerField.Item;
			image.Field = field.InnerField.ID.ToString();
		}
	}
}
