using Sitecore.ContentSearch.Linq.Common;
using Synthesis.Templates;
using Synthesis.Generation;
using Synthesis.FieldTypes;
using Synthesis.Initializers;
using Synthesis.Synchronization;

namespace Synthesis.Configuration
{
	public interface IProviderConfiguration
	{
		string Name { get; }
		IFieldMappingProvider FieldMappingProvider { get; }
		IGeneratorParametersProvider GeneratorParametersProvider { get; }
		ITemplateInputProvider TemplateInputProvider { get; }
		ITemplateSignatureProvider TemplateSignatureProvider { get; }
		ITypeListProvider TypeListProvider { get; }
		IInitializerProvider InitializerProvider { get; }
		FieldNameTranslator IndexFieldNameTranslator { get; }
		IMetadataGenerator CreateMetadataGenerator();
		ITemplateCodeGenerator CreateCodeGenerator();
		SynchronizationEngine CreateSyncEngine();
	}
}
