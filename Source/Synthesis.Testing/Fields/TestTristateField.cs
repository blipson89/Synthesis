using System.Linq;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Testing.Fields
{
	/// <summary>
	/// Encapsulates a tri-state (Yes/No/Default) field from Sitecore
	/// </summary>
	public class TestTristateField : TestFieldType, ITristateField
	{
		public TestTristateField(bool? value)
		{
			Value = value;
		}

		/// <summary>
		/// Gets the value of the field. If called when HasValue is false, returns false.
		/// </summary>
		public bool? Value { get; set; }

		/// <summary>
		/// Checks if this field has a value. Note that this is always true for a tristate field because no value is still a valid value ("default").
		/// </summary>
		public override bool HasValue
		{
			get { return true; }
		}
	}
}
