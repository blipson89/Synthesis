using Synthesis.ContentSearch;
using Synthesis.Templates;
using Synthesis.Generation;
using Synthesis.FieldTypes;

namespace Synthesis.Configuration
{
	public interface IProviderConfiguration
	{
		IFieldMappingProvider FieldMappingProvider { get; }
		IGeneratorParametersProvider GeneratorParametersProvider { get; }
		ITemplateInputProvider TemplateInputProvider { get; }
		ITemplateSignatureProvider TemplateSignatureProvider { get; }
		ITypeListProvider TypeListProvider { get; }
		ISynthesisIndexFieldNameTranslator IndexFieldNameTranslator { get; }
	}
}
