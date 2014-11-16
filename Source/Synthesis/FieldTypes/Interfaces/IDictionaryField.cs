namespace Synthesis.FieldTypes.Interfaces
{
	public interface IDictionaryField : IFieldType
	{
		/// <summary>
		/// Gets the set of IDs that make up the relationships
		/// </summary>
		string this[string key] { get; set; }

		int Count { get; }

		void Add(string key, string value);
		bool Remove(string key);
		void Clear();
		bool ContainsKey(string key);
	}
}