namespace Synthesis.ContentSearch
{
	/// <summary>
	/// Tells the Synthesis generator how to mark fields for index queries with the [IndexField] attribute
	/// </summary>
	public interface ISynthesisIndexFieldNameTranslator
	{
		string GetIndexFieldName(string fieldName);
	}
}
