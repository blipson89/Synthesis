using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sitecore.ContentSearch;

namespace Synthesis.ContentSearch
{
	/// <summary>
	/// This translates field names into the index the same way Synthesis generates them for objects.
	/// </summary>
	public class SynthesisFieldNameTranslator : AbstractFieldNameTranslator
	{
		private readonly AbstractFieldNameTranslator _innerTranslator;

		public SynthesisFieldNameTranslator(AbstractFieldNameTranslator innerTranslator)
		{
			_innerTranslator = innerTranslator;
		}
		
		public override string GetIndexFieldName(MemberInfo member)
		{
			return _innerTranslator.GetIndexFieldName(member);
		}

		public override string GetIndexFieldName(string fieldName, Type returnType)
		{
			return _innerTranslator.GetIndexFieldName(PreProcessSynthesisFieldName(fieldName), returnType);
		}

		public override string GetIndexFieldName(string fieldName)
		{
			return _innerTranslator.GetIndexFieldName(PreProcessSynthesisFieldName(fieldName));
		}

		public override IEnumerable<string> GetTypeFieldNames(string fieldName)
		{
			return _innerTranslator.GetTypeFieldNames(fieldName);
		}

		public override Dictionary<string, List<string>> MapDocumentFieldsToType(Type type, MappingTargetType target, IEnumerable<string> documentFieldNames)
		{
			return _innerTranslator.MapDocumentFieldsToType(type, target, documentFieldNames);
		}

		protected virtual string PreProcessSynthesisFieldName(string fieldName)
		{
			return fieldName.Split('.').First();
		}
	}
}
