using System.Linq;
using System.Globalization;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Testing.Fields
{
	public class TestIntegerField : TestFieldType, IIntegerField
	{
		private int? _value;

		public TestIntegerField(int? value)
		{
			_value = value;
		}

		/// <summary>
		/// Gets the value of the field. If the field does not have a value or the value isn't parseable, returns default(int).
		/// </summary>
		public virtual int Value 
		{ 
			get { return _value ?? default(int); }
			set { _value = value; }
		}

		/// <summary>
		/// Renders the field using a Sitecore FieldRenderer and returns the result
		/// </summary>
		public virtual string RenderedValue
		{
			get { return Value.ToString(CultureInfo.InvariantCulture); }
		}

		/// <summary>
		/// Checks if the field has a valid integer value
		/// </summary>
		/// <remarks>Note that Value will always return a valid int (0) even if HasValue is false. So check this first :)</remarks>
		public override bool HasValue
		{
			get { return _value.HasValue; }
		}
	}
}
