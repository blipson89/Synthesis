using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.ContentSearch.SolrProvider;
using Sitecore.Data;
using Sitecore.Globalization;
using Synthesis.FieldTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Synthesis.Solr.ContentSearch
{
	class SynthesisSolrFieldNameTranslator : FieldNameTranslator
	{
		private SolrSearchContext _context;

		public SynthesisSolrFieldNameTranslator(SolrSearchContext context)
		{
			this._context = context;
		}
		/// <summary>
		/// The field mapper wants to append methods onto the SOLR field (I.E. _temlate.rawvalue) by returning null in the member method
		/// below for the raw value method and removing the dot here we can correct this behavior
		/// </summary>
		/// <param name="fieldName">The name of the solr field thus far in the process</param>
		/// <returns>solr field name</returns>
		public override string GetIndexFieldName(string fieldName)
		{
			if (fieldName.EndsWith("."))
				return fieldName.Remove(fieldName.Length - 1);
			if (_context.Index.Schema.AllFieldNames.Where(x => x == fieldName).Any())
				return fieldName;
			fieldName = AppendSolrText(fieldName);
			return fieldName;
		}


		/// <summary>
		/// Take the property from a synthesis member and translate that into a solr field
		/// </summary>
		/// <param name="member">Class member that corresponds to a synthesis object property</param>
		/// <returns>solr field name</returns>
		public override string GetIndexFieldName(MemberInfo member)
		{
			if (!member.CustomAttributes.Any() || !member.CustomAttributes.Where(a => a.AttributeType == typeof(Sitecore.ContentSearch.IndexFieldAttribute)).Any())
				return null;
			var attributes = member.CustomAttributes.Where(a => a.AttributeType == typeof(Sitecore.ContentSearch.IndexFieldAttribute));
			if (!attributes.Any())
				return null;
			var fieldName = attributes.First().ConstructorArguments.First().Value.ToString();
			if (_context.Index.Schema.AllFieldNames.Where(x => x == fieldName).Any())
				return fieldName;
			Type type = ((PropertyInfo)member).PropertyType;
			if (typeof(ITextField).IsAssignableFrom(type)
				|| typeof(IHyperlinkField).IsAssignableFrom(type)
				|| typeof(string).IsAssignableFrom(type))
				AppendSolrText(fieldName);
			else if (typeof(IBooleanField).IsAssignableFrom(type)
				|| typeof(bool).IsAssignableFrom(type))
				fieldName += "_b";
			else if (typeof(IItemReferenceListField).IsAssignableFrom(type)
				|| typeof(IDictionaryField).IsAssignableFrom(type)
				|| typeof(ID[]).IsAssignableFrom(type))
				fieldName += "_sm";
			else if (typeof(IDateTimeField).IsAssignableFrom(type)
				|| typeof(DateTime).IsAssignableFrom(type))
				fieldName += "_tdt";
			else if (typeof(IIntegerField).IsAssignableFrom(type)
				|| typeof(int).IsAssignableFrom(type))
				fieldName += "_i";
			return fieldName;
		}
		/// <summary>
		/// If the context is a foreign language we should use the foreign language text solr fields
		/// </summary>
		/// <param name="fieldName">the initial field name</param>
		/// <returns>field name with a dynamic field identifier on it</returns>
		private string AppendSolrText(string fieldName)
		{
			if (Sitecore.Context.Site == null || Sitecore.Context.Language.Name == Sitecore.Context.Site.Language)
				fieldName += "_t";
			else
				fieldName += "_t_" + Sitecore.Context.Language;
			return fieldName;
		}
	}
}
