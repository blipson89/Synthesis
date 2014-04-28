using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.LuceneProvider;

namespace Synthesis.ContentSearch.Lucene
{
	/// <summary>
	/// Plugs Synthesis' field-naming conventions into the standard index
	/// </summary>
	public class SynthesisLinqToLuceneIndex<TItem> : LinqToLuceneIndex<TItem>
	{
		private readonly FieldNameTranslator _fieldNameTranslator;

		public SynthesisLinqToLuceneIndex(LuceneSearchContext context) : this(context, null)
		{
			
		}

		public SynthesisLinqToLuceneIndex(LuceneSearchContext context, params IExecutionContext[] executionContexts)
			: base(context, executionContexts)
		{
			_fieldNameTranslator = new SynthesisFieldNameTranslator(context.Index.FieldNameTranslator);
		}

		protected override FieldNameTranslator FieldNameTranslator
		{
			get { return _fieldNameTranslator; }
		}
	}
}
