using Sitecore.ContentSearch.LuceneProvider;

namespace Synthesis.ContentSearch.Lucene
{
	/// <summary>
	/// This translates field names into the index the same way Synthesis generates them for objects.
	/// Unused at present, but later versions of Sitecore 7 may require this behaviour
	/// </summary>
	public class SynthesisLuceneFieldNameTranslator : LuceneFieldNameTranslator
	{
		readonly static ISynthesisIndexFieldNameTranslator InnerSynthesisTranslator = new SynthesisLuceneIndexFieldNameTranslator();

		public SynthesisLuceneFieldNameTranslator(ILuceneProviderIndex index) : base(index)
		{
			
		}

		public override string GetIndexFieldName(string fieldName)
		{
			return InnerSynthesisTranslator.GetIndexFieldName(fieldName);
		}
	}
}
