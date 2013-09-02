using System;
using System.Globalization;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.FieldTypes
{
	public class IntegerField : FieldType, IIntegerField
	{
		public IntegerField(Lazy<Field> field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the value of the field. If the field does not have a value or the value isn't parseable, returns default(int).
		/// </summary>
		public virtual int Value 
		{ 
			get 
			{
				int value;
				if (int.TryParse(InnerField.Value, out value)) return value;

				return default(int);
			}
			set { SetFieldValue(value.ToString(CultureInfo.InvariantCulture)); }
		}

		/// <summary>
		/// Renders the field using a Sitecore FieldRenderer and returns the result
		/// </summary>
		public virtual string RenderedValue
		{
			get { return FieldRenderer.Render(InnerItem, InnerField.ID.ToString()); }
		}

		/// <summary>
		/// Checks if the field has a valid integer value
		/// </summary>
		/// <remarks>Note that Value will always return a valid int (0) even if HasValue is false. So check this first :)</remarks>
		public override bool HasValue
		{
			get 
			{
				if (InnerField == null) return false;

				int value;
				return int.TryParse(InnerField.Value, out value);
			}
		}

		public override string ToString()
		{
			return Value.ToString(CultureInfo.InvariantCulture);
		}

		public static implicit operator int(IntegerField field)
		{
			return field.Value;
		}

		public static implicit operator string(IntegerField field)
		{
			return (field.HasValue) ? field.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
		}
	}
}
