using System;
using Sitecore.Data.Fields;

namespace Synthesis.FieldTypes
{
	/// <summary>
	/// Encapsulates a boolean (checkbox) field from Sitecore
	/// </summary>
	public class BooleanField : FieldType
	{
		public BooleanField(Lazy<Field> field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the value of the field. If called when HasValue is false, returns false.
		/// </summary>
		public virtual bool Value 
		{ 
			get { return ((CheckboxField)InnerField).Checked; } 
			set 
			{
				SetFieldValue(delegate {
					((CheckboxField)InnerField).Checked = value;
				});
			} 
		}

		/// <summary>
		/// Checks if this field has a value. Note that this is always true for a boolean field.
		/// </summary>
		public override bool HasValue
		{
			get { return true; }
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public static implicit operator bool(BooleanField field)
		{
			return field.Value;
		}

		public static implicit operator CheckboxField(BooleanField field)
		{
			return field.InnerField;
		}
	}
}
