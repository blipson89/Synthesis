namespace Synthesis.ContentSearch.Lucene
{
	/// <remarks>
	/// Originally this design used the Sitecore.ContentSearch.AbstractFieldNameTranslator.
	/// However these classes require an index context, which we don't have, and all we need is string translation.
	/// </remarks>
	public class SynthesisLuceneIndexFieldNameTranslator : ISynthesisIndexFieldNameTranslator
	{
		public string GetIndexFieldName(string fieldName)
		{
			// this code "smashes" sub-property field names down into their parent field
			// this allows Synthesis to automap eg "x.TextField.RawValue" onto the "textfield" lucene index field, instead of "textfield.rawvalue" as the expression parser constructs
			// code courtesy of Martin Hyldahl, a man of badassery.
			// NOTE: this is a feature of a prerelease version of the expression parser and is included here only as a future compatibility measure
			int nestedMemberDelimiterIdx = fieldName.IndexOf('.');

			if (nestedMemberDelimiterIdx >= 0)
				fieldName = fieldName.Substring(0, nestedMemberDelimiterIdx);

			// default provider behavior
			fieldName = fieldName.Replace(" ", "_");

			return fieldName.ToLowerInvariant();
		}
	}
}
