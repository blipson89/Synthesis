using Sitecore.Data.Fields;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.FieldTypes
{
	/// <summary>
	/// Encapsulates a tri-state (Yes/No/Default) field from Sitecore
	/// </summary>
	public class TristateField : FieldType, ITristateField
	{
		public TristateField(LazyField field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the value of the field. If called when HasValue is false, returns false.
		/// </summary>
		public virtual bool? Value 
		{ 
			get 
			{
				if (InnerField.Value == "1") return true;
				if (InnerField.Value == "0") return false;

				return null;
			} 
			set 
			{
				SetFieldValue(delegate
				{
					if (value == null)
						InnerField.Value = string.Empty;
					else
						((CheckboxField)InnerField).Checked = value.Value;
				});
			} 
		}

		/// <summary>
		/// Checks if this field has a value. Note that this is always true for a tristate field because no value is still a valid value ("default").
		/// </summary>
		public override bool HasValue
		{
			get { return true; }
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}
}
