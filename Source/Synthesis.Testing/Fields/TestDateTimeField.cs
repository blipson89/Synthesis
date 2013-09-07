using System.Linq;
using System;
using Sitecore.Data.Fields;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Testing.Fields
{
	public class TestDateTimeField : TestFieldType, IDateTimeField
	{
		public TestDateTimeField(DateTime dateTime)
		{
			Value = dateTime;
		}

		/// <summary>
		/// Gets the value of the field. If no value exists, returns DateTime.MinValue
		/// </summary>
		public DateTime Value { get; private set; }

		/// <summary>
		/// Checks if the field has a value. For a DateTime, this checks if it equals default(DateTime).
		/// </summary>
		public override bool HasValue
		{
			get
			{
				return Value != DateTime.MinValue;
			}
		}

		/// <summary>
		/// Renders the field using a Sitecore FieldRenderer and returns the result
		/// </summary>
		public virtual string RenderedValue
		{
			get { return Value.ToString(); }
		}

		public DateField ToDateField()
		{
			throw new NotImplementedException("Test date fields cannot return Sitecore item objects");
		}
	}
}
