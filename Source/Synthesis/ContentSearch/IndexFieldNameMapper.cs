using Synthesis.Templates;

namespace Synthesis.ContentSearch
{
	/// <summary>
	/// Maps a template field name to a search index field name
	/// Not using FieldNameTranslator because that tries to get too fancy for Solr,
	/// mapping e.g. [IndexField("foo_t")] when [IndexField("foo")] would do.
	/// </summary>
	public class IndexFieldNameMapper
	{
		public virtual string MapToSearchField(ITemplateFieldInfo field)
		{
			// this seems to work just fine across Solr and Lucene
			// using something else? Patch in a different one.
			return field.Name.Replace(" ", "_").ToLowerInvariant();
		}
	}
}
