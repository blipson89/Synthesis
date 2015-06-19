using System.Linq;
using System.Reflection;
using Sitecore;
using Sitecore.ContentSearch;
using SolrNet.Schema;
using Synthesis.ContentSearch;

namespace Synthesis.Solr.ContentSearch
{
	public class SynthesisSolrFieldNameTranslator : SynthesisFieldNameTranslator
	{
		private readonly SolrSchema _schema;
		public SynthesisSolrFieldNameTranslator(IProviderSearchContext context, AbstractFieldNameTranslator translator)
			: base(translator)
		{
			//sitecore hides the solr schema behind a private field, we need it to find dynamic fields.
			var fieldInfo = context.Index.Schema.GetType().GetField("schema", BindingFlags.NonPublic | BindingFlags.Instance);
			if (fieldInfo != null)
				_schema = (SolrSchema)fieldInfo.GetValue(context.Index.Schema);
		}

		public override string GetIndexFieldName(string fieldName)
		{
			if (_schema != null && _schema.FindSolrFieldByName(fieldName) != null || _schema.SolrDynamicFields.Any(x => fieldName.EndsWith(x.Name.Substring(1))))
				return fieldName;
			//at this point we can't be sure what type the data is in the field, our best bet would be a text field.
			return AppendSolrText(fieldName);
		}
		/// <summary>
		/// If the context is a foreign language we should use the foreign language text solr fields
		/// </summary>
		/// <param name="fieldName">the initial field name</param>
		/// <returns>field name with a dynamic field identifier on it</returns>
		private string AppendSolrText(string fieldName)
		{
			if (Context.Site == null || Context.Language.Name == Context.Site.Language)
				fieldName += "_t";
			else
				fieldName += "_t_" + Context.Language;
			return fieldName;
		}

	}
}
