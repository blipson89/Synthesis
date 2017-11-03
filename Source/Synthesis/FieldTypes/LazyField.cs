using System;
using Sitecore.Data.Fields;

namespace Synthesis.FieldTypes
{
	public class LazyField
	{
		private readonly string _fieldName;
		private readonly string _templateName;
		private readonly Lazy<Field> _innerLazyField; 

		public LazyField(Func<Field> valueFactory, string templateName, string fieldName)
		{
			_innerLazyField = new Lazy<Field>(valueFactory);
			_templateName = templateName;
			_fieldName = fieldName;
		}

		public bool IsLoaded => _innerLazyField.IsValueCreated;

		public Field Value
		{
			get
			{
				var value = _innerLazyField.Value;
				if (value == null) throw new MissingTemplateFieldException($"The field {_fieldName} on template {_templateName} was not found. It may be unpublished, missing, or you may need to regenerate your Synthesis model.");

				return value;
			}
		}
	}
}
