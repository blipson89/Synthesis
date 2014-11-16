using Synthesis.FieldTypes;
using Synthesis.FieldTypes.Interfaces;
using Sitecore.Web.UI.WebControls;
using System.Web.UI.WebControls;
using System.Diagnostics.CodeAnalysis;

namespace Synthesis
{
	public static class WebControlExtensions
	{
		/// <summary>
		/// Attaches a Synthesis date field to a Sitecore Date web control
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Doesn't make semantic sense")]
		public static void AttachToDateTimeField(this Date date, IDateTimeField field)
		{
			var item = (FieldType)field;
			date.Item = item.InnerField.Item;
			date.Field = field.Id.ToString();
		}

		/// <summary>
		/// Attaches the HyperLink web control to a FileField as a download link. The control is set to not visible if the media item does not have a value and we aren't page editing. 
		/// </summary>
		public static void AttachToFileField(this HyperLink link, IFileField field)
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
		public static void AttachToLinkField(this Link link, IHyperlinkField field)
		{
			var item = (FieldType)field;
			link.Item = item.InnerField.Item;
			link.Field = item.InnerField.ID.ToString();
		}

		/// <summary>
		/// Attaches the image to a Synthesis ImageField. The control is set to not visible if the media item does not have a value and we aren't page editing.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Doesn't make semantic sense")]
		public static void AttachToImageField(this Sitecore.Web.UI.WebControls.Image image, IImageField field)
		{
			if (!field.HasValue && !Sitecore.Context.PageMode.IsPageEditor)
			{
				image.Visible = false;
				return;
			}

			var item = (FieldType)field;
			image.Item = item.InnerField.Item;
			image.Field = item.InnerField.ID.ToString();
		}
	}
}
