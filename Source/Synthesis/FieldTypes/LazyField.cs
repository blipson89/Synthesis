using System;
using System.Linq;
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

		public bool IsLoaded { get { return _innerLazyField.IsValueCreated; }}

		public Field Value
		{
			get
			{
				var value = _innerLazyField.Value;
				if (value == null) throw new MissingTemplateFieldException(string.Format("The field {0} on template {1} was not found. It may be unpublished, missing, or you may need to regenerate your Synthesis model.", _fieldName, _templateName));

				return value;
			}
		}
	}
}
