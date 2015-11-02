﻿using System;
using System.Globalization;
using Sitecore;
using Sitecore.ContentSearch.Converters;
using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.FieldTypes
{
	public class DateTimeField : FieldType, IDateTimeField
	{
		public DateTimeField(LazyField field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the value of the field. If no value exists, returns DateTime.MinValue
		/// </summary>
		public virtual DateTime Value
		{
			get
			{
				if (!IsFieldLoaded && InnerSearchValue != null)
				{
					DateTime ret;
					if (DateTime.TryParse(InnerSearchValue, out ret))
						return ret;
					if (DateTime.TryParseExact(InnerSearchValue, "yyyyMMdd'T'HHmm'Z'", CultureInfo.CurrentCulture,
						DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out ret))
						return ret;
					var converter = new IndexFieldDateTimeValueConverter();
					//// ReSharper disable once PossibleNullReferenceException
					return (DateTime)converter.ConvertFrom(InnerSearchValue);
				}

				return ((DateField)InnerField).DateTime;
			}
			set { SetFieldValue(DateUtil.ToIsoDate(value)); }
		}

		/// <summary>
		/// Checks if the field has a value. For a DateTime, this checks if it equals default(DateTime).
		/// </summary>
		public override bool HasValue
		{
			get
			{
				if (InnerField == null) return false;

				return Value != DateTime.MinValue;
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

		public override string ToString()
		{
			return Value.ToString(CultureInfo.InvariantCulture);
		}

		public DateField ToDateField()
		{
			return InnerField;
		}
	}
}
