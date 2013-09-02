using System.Globalization;
using Sitecore.Web.UI.WebControls;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.FieldTypes
{
	public class ImageField : FileField, IImageField
	{
		private int? _width, _height;

		public ImageField(LazyField field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the width of the image, if one was entered
		/// </summary>
		public virtual int? Width
		{
			get
			{
				if (_width != null) return _width;

				int width;
				if (int.TryParse(((Sitecore.Data.Fields.ImageField)InnerField).Width, out width)) return width;

				return null;
			}
			set
			{
				SetFieldValue(delegate
				{
					((Sitecore.Data.Fields.ImageField)InnerField).Width = (value == null) ? string.Empty : value.Value.ToString(CultureInfo.InvariantCulture);
					_width = null;
				});
			}
		}

		/// <summary>
		/// Gets the height of the image, if one was entered
		/// </summary>
		public virtual int? Height
		{
			get
			{
				if (_height != null) return _height;

				int height;
				if (int.TryParse(((Sitecore.Data.Fields.ImageField)InnerField).Height, out height)) return height;

				return null;
			}
			set 
			{
				SetFieldValue(delegate
				{
					((Sitecore.Data.Fields.ImageField)InnerField).Height = (value == null) ? string.Empty : value.Value.ToString(CultureInfo.InvariantCulture);
					_height = null;
				});
			}
		}

		/// <summary>
		/// Gets the alt text of the image, if any was entered
		/// </summary>
		public virtual string AlternateText
		{
			get { return ((Sitecore.Data.Fields.ImageField)InnerField).Alt; }
			set
			{
				SetFieldValue(delegate
				{
					((Sitecore.Data.Fields.ImageField)InnerField).Alt = value;
				});
			}
		}

		/// <summary>
		/// Renders the field using a Sitecore FieldRenderer and returns the result
		/// </summary>
		public virtual string RenderedValue
		{
			get
			{
				return FieldRenderer.Render(InnerItem, InnerField.ID.ToString());
			}
		}
	}
}
