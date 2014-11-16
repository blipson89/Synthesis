using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Testing
{
	/// <summary>
	///     Encapsulates a boolean (checkbox) field from Sitecore
	/// </summary>
	public class TestBooleanField : TestFieldType, IBooleanField
	{
		public TestBooleanField(bool value)
		{
			Value = value;
		}

		public bool Value { get; set; }

		public override bool HasValue
		{
			get { return true; }
		}
	}
}