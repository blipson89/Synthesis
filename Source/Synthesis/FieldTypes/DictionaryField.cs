using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.FieldTypes
{
	/// <summary>
	/// Represents a field that is a logical dictionary (i.e. a Key Value List)
	/// </summary>
	public class DictionaryField : FieldType, IDictionaryField
	{
		NameValueCollection _values;

		public DictionaryField(LazyField field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the set of IDs that make up the relationships
		/// </summary>
		public virtual string this[string key]
		{
			get { return Values[key]; }
			set { Values[key] = value; }
		}

		/// <summary>
		/// Gets the keys and values in the field
		/// </summary>
		public virtual NameValueCollection Values
		{
			get
			{
				if (_values == null)
				{
					_values = HttpUtility.ParseQueryString(InnerField.Value);
				}

				return _values;
			}
		}

		/// <summary>
		/// Checks if the field has at least one key
		/// </summary>
		public override bool HasValue
		{
			get
			{
				if (InnerField == null) return false; 
				return Values.Count > 0;
			}
		}

		public void Add(string key, string value)
		{
			Values.Add(key, value);
			
			SetFieldValue(ValueToQueryString());
		}

		public bool Remove(string key)
		{
			if (!ContainsKey(key)) return false;

			Values.Remove(key);
			SetFieldValue(ValueToQueryString());

			return true;
		}

		public void Clear()
		{
			SetFieldValue(string.Empty);
			_values = null;
		}

		public bool ContainsKey(string key)
		{
			return Values[key] != null;
		}

		public int Count
		{
			get { return Values.Count; }
		}

		private string ValueToQueryString()
		{
			 var items = new List<string>();
 
			foreach (string name in Values)
				items.Add(string.Concat(Uri.EscapeDataString(name), "=", Uri.EscapeDataString(Values[name])));
 
			return string.Join("&", items);
		}
	}
}
