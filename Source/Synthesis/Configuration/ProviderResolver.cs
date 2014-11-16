using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.Diagnostics;
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

		/// <summary>
		/// Gets the currently configured Field Mapping Provider (maps a Sitecore field type to an implementing .NET class)
		/// </summary>
		public IFieldMappingProvider FieldMappingProvider
		{
			get
			{
				if (_fieldMappingProvider == null) _fieldMappingProvider = LoadFieldMappingProviderFromConfig();
				return _fieldMappingProvider;
			}
		}
		private IFieldMappingProvider _fieldMappingProvider;

		/// <summary>
		/// Gets the currently configured Generator Parameters Provider (configures the Synthesis Generator's parameters)
		/// </summary>
		public IGeneratorParametersProvider GeneratorParametersProvider
		{
			get
			{
				if (_generatorParametersProvider == null) _generatorParametersProvider = LoadGeneratorParametersProviderFromConfig();
				return _generatorParametersProvider;
			}
		}
		private IGeneratorParametersProvider _generatorParametersProvider;

		/// <summary>
		/// Gets the currently configured Template Input Provider (configures the set of templates and fields that should be acted upon)
		/// </summary>
		public ITemplateInputProvider TemplateInputProvider
		{
			get
			{
				if (_templateInputProvider == null) _templateInputProvider = LoadTemplateInputProviderFromConfig();
				return _templateInputProvider;
			}
		}
		private ITemplateInputProvider _templateInputProvider;

		/// <summary>
		/// Gets the currently configured Template Signature Provider (provides a unique signature for a template's state for later comparison)
		/// </summary>
		public ITemplateSignatureProvider TemplateSignatureProvider
		{
			get
			{
				if (_templateSignatureProvider == null) _templateSignatureProvider = LoadTemplateSignatureProviderFromConfig();
				return _templateSignatureProvider;
			}
		}
		private ITemplateSignatureProvider _templateSignatureProvider;

		/// <summary>
		/// Gets the currently configured Type List Provider (configures where Synthesis looks for Synthesis types to be defined and for presenters)
		/// </summary>
		public ITypeListProvider TypeListProvider
		{
			get
			{
				if (_typeListProvider == null) _typeListProvider = LoadTypeListProviderFromConfig();
				return _typeListProvider;
			}
		}
		private ITypeListProvider _typeListProvider;

		/// <summary>
		/// Gets the currently configured FieldNameTranslator (converts Sitecore template field names to index field names)
		/// </summary>
		public FieldNameTranslator IndexFieldNameTranslator
		{
			get
			{
				if (_fieldNameTranslator == null) _fieldNameTranslator = LoadFieldNameTranslatorFromConfig();
				return _fieldNameTranslator;
			}
		}
		private FieldNameTranslator _fieldNameTranslator;

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

		private static FieldNameTranslator LoadFieldNameTranslatorFromConfig()
		{
			var indexConfiguration = Factory.GetConfigNode("/sitecore/synthesis/providers/indexConfiguration", true);

			// ReSharper disable once PossibleNullReferenceException
			var nameAttribute = indexConfiguration.Attributes["name"];

			Assert.IsNotNull(nameAttribute, "The index name was missing on the Synthesis index configuration.");

			return ContentSearchManager.GetIndex(nameAttribute.InnerText).FieldNameTranslator;
		}
	}
}
