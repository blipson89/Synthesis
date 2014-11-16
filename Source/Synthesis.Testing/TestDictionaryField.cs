using System.Collections.Specialized;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Testing
{
	/// <summary>
	///     Represents a field that is a logical dictionary (i.e. a Key Value List)
	/// </summary>
	public class TestDictionaryField : TestFieldType, IDictionaryField
	{
		private NameValueCollection _values;

		public TestDictionaryField(NameValueCollection values)
		{
			_values = values ?? new NameValueCollection();
		}

		/// <summary>
		///     Gets the set of IDs that make up the relationships
		/// </summary>
		public string this[string key]
		{
			get { return _values[key]; }
			set { _values[key] = value; }
		}

		/// <summary>
		///     Checks if the field has at least one key
		/// </summary>
		public override bool HasValue
		{
			get { return _values.Count > 0; }
		}

		public void Add(string key, string value)
		{
			_values.Add(key, value);
		}

		public bool Remove(string key)
		{
			if (!ContainsKey(key)) return false;

			_values.Remove(key);

			return true;
		}

		public void Clear()
		{
			_values = new NameValueCollection();
		}

		public bool ContainsKey(string key)
		{
			return _values[key] != null;
		}

		public int Count
		{
			get { return _values.Count; }
		}
	}
}