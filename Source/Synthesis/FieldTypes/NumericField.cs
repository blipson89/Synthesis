using System;
using System.Globalization;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.FieldTypes
{
	public class NumericField : FieldType, INumericField
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
				if (InnerField == null) return false;

				decimal value;
				return decimal.TryParse(InnerField.Value, out value);
			}
		}

		public override string ToString()
		{
			return Value.ToString(CultureInfo.InvariantCulture);
		}
	}
}
