using System;
using System.Globalization;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;

namespace Synthesis.FieldTypes
{
	public class NumericField : FieldType, IFieldRenderableFieldType
	{
		public NumericField(Lazy<Field> field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the value of the field. If the field does not have a value, default(decimal) is returned instead.
		/// </summary>
		public virtual decimal Value
		{
			get
			{
				decimal value;
				if (decimal.TryParse(InnerField.Value, out value)) return value;

				return default(decimal);	
			}
			set { SetFieldValue(value.ToString(CultureInfo.InvariantCulture)); }
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

		/// <summary>
		/// Checks if the field has a value
		/// </summary>
		public override bool HasValue
		{
			get 
			{
				decimal value;
				return decimal.TryParse(InnerField.Value, out value);
			}
		}

		public override string ToString()
		{
			return Value.ToString(CultureInfo.InvariantCulture);
		}

		public static implicit operator decimal(NumericField field)
		{
			return field.Value;
		}

		public static implicit operator string(NumericField field)
		{
			return field.HasValue ? field.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
		}
	}
}
