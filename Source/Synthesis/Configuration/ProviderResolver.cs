using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Synthesis.ContentSearch;
using Synthesis.FieldTypes;
using Synthesis.Generation;
using Synthesis.Synchronization;
using Synthesis.Templates;

namespace Synthesis.Configuration
{
	public sealed class ProviderResolver : IProviderConfiguration
	{
		private static readonly object SyncRoot = new object();
		private static volatile IProviderConfiguration _currentManager;

		/// <summary>
		/// Gets the default Provider Manager instance (resolves provider types from XML configuration)
		/// </summary>
		public static IProviderConfiguration Current
		{
			get
			{
				if (_currentManager == null)
				{
					lock (SyncRoot)
					{
						if (_currentManager == null)
							_currentManager = new ProviderResolver();
					}
				}

				return _currentManager;
			}

			set
			{
				lock (SyncRoot)
				{
					_currentManager = value;
				}
			}
		}

		// default instance reads values from XML configuration
		private ProviderResolver()
		{
			FieldMappingProvider = LoadFieldMappingProviderFromConfig();
			GeneratorParametersProvider = LoadGeneratorParametersProviderFromConfig();
			TemplateInputProvider = LoadTemplateInputProviderFromConfig();
			TemplateSignatureProvider = LoadTemplateSignatureProviderFromConfig();
			TypeListProvider = LoadTypeListProviderFromConfig();
			IndexFieldNameTranslator = LoadFieldNameTranslatorFromConfig();
		}

		/// <summary>
		/// Gets the currently configured Field Mapping Provider (maps a Sitecore field type to an implementing .NET class)
		/// </summary>
		public IFieldMappingProvider FieldMappingProvider { get; private set; }

		/// <summary>
		/// Gets the currently configured Generator Parameters Provider (configures the Synthesis Generator's parameters)
		/// </summary>
		public IGeneratorParametersProvider GeneratorParametersProvider { get; private set; }

		/// <summary>
		/// Gets the currently configured Template Input Provider (configures the set of templates and fields that should be acted upon)
		/// </summary>
		public ITemplateInputProvider TemplateInputProvider { get; private set; }

		/// <summary>
		/// Gets the currently configured Template Signature Provider (provides a unique signature for a template's state for later comparison)
		/// </summary>
		public ITemplateSignatureProvider TemplateSignatureProvider { get; private set; }

		/// <summary>
		/// Gets the currently configured Type List Provider (configures where Synthesis looks for Synthesis types to be defined and for presenters)
		/// </summary>
		public ITypeListProvider TypeListProvider { get; private set; }
		
		/// <summary>
		/// Gets the currently configured FieldNameTranslator (converts Sitecore template field names to index field names)
		/// </summary>
		public ISynthesisIndexFieldNameTranslator IndexFieldNameTranslator { get; private set; }

		/// <summary>
		/// Gets an instance of the Synthesis Generator pre-configured to use the current provider set
		/// </summary>
		public static Generator CreateGenerator()
		{
			return new Generator(Current.GeneratorParametersProvider, Current.TemplateInputProvider, Current.TemplateSignatureProvider, Current.FieldMappingProvider, Current.IndexFieldNameTranslator);
		}

		/// <summary>
		/// Gets an instance of the Synthesis Synchronization Engine pre-configured to use the current provider set
		/// </summary>
		/// <returns></returns>
		public static SynchronizationEngine CreateSyncEngine()
		{
			return new SynchronizationEngine(Current.TemplateSignatureProvider, Current.TemplateInputProvider, Current.TypeListProvider);
		}

		private static IFieldMappingProvider LoadFieldMappingProviderFromConfig()
		{
			// Sitecore has some serious jedi config tricks like this one
			return (IFieldMappingProvider)Factory.CreateObject("/sitecore/synthesis/providers/fieldMappingProvider", true);
		}

		private static IGeneratorParametersProvider LoadGeneratorParametersProviderFromConfig()
		{
			return (IGeneratorParametersProvider)Factory.CreateObject("/sitecore/synthesis/providers/generatorParametersProvider", true);
		}

		private static ITemplateInputProvider LoadTemplateInputProviderFromConfig()
		{
			return (ITemplateInputProvider)Factory.CreateObject("/sitecore/synthesis/providers/templateInputProvider", true);
		}

		private static ITemplateSignatureProvider LoadTemplateSignatureProviderFromConfig()
		{
			return (ITemplateSignatureProvider)Factory.CreateObject("/sitecore/synthesis/providers/templateSignatureProvider", true);
		}

		private static ITypeListProvider LoadTypeListProviderFromConfig()
		{
			return (ITypeListProvider)Factory.CreateObject("/sitecore/synthesis/providers/typeListProvider", true);
		}

		private static ISynthesisIndexFieldNameTranslator LoadFieldNameTranslatorFromConfig()
		{
			return (ISynthesisIndexFieldNameTranslator)Factory.CreateObject("/sitecore/synthesis/providers/indexFieldNameTranslator", true);
		}
	}
}
