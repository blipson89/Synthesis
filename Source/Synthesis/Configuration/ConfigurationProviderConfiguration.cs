using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.Diagnostics;
using Synthesis.FieldTypes;
using Synthesis.Generation;
using Synthesis.Generation.CodeDom;
using Synthesis.Initializers;
using Synthesis.Synchronization;
using Synthesis.Templates;

namespace Synthesis.Configuration
{
	public class ConfigurationProviderConfiguration : IProviderConfiguration
	{
		public ConfigurationProviderConfiguration()
		{
			// ReSharper disable once DoNotCallOverridableMethodsInConstructor
			Name = "Default Configuration";
		}

		public virtual string Name { get; set; }

		/// <summary>
		/// Gets the currently configured Field Mapping Provider (maps a Sitecore field type to an implementing .NET class)
		/// </summary>
		public virtual IFieldMappingProvider FieldMappingProvider
		{
			get
			{
				if (_fieldMappingProvider == null)
				{
					lock (_fieldMappingProviderInitLock)
					{
						if (_fieldMappingProvider == null)
						{
							_fieldMappingProvider = LoadFieldMappingProviderFromConfig();
						}
					}
				}
				return _fieldMappingProvider;
			}
			set { _fieldMappingProvider = value; }
		}
		private IFieldMappingProvider _fieldMappingProvider;
		private readonly object _fieldMappingProviderInitLock = new object();

		/// <summary>
		/// Gets the currently configured Generator Parameters Provider (configures the Synthesis Generator's parameters)
		/// </summary>
		public virtual IGeneratorParametersProvider GeneratorParametersProvider
		{
			get
			{
				if (_generatorParametersProvider == null)
				{
					lock (_generatorParametersProviderInitLock)
					{
						if (_generatorParametersProvider == null)
						{
							_generatorParametersProvider = LoadGeneratorParametersProviderFromConfig();
						}
					}
				}
				return _generatorParametersProvider;
			}
		}
		private IGeneratorParametersProvider _generatorParametersProvider;
		private readonly object _generatorParametersProviderInitLock = new object();

		/// <summary>
		/// Gets the currently configured Template Input Provider (configures the set of templates and fields that should be acted upon)
		/// </summary>
		public virtual ITemplateInputProvider TemplateInputProvider
		{
			get
			{
				if (_templateInputProvider == null)
				{
					lock (_templateInputInitLock)
					{
						if (_templateInputProvider == null)
						{
							_templateInputProvider = LoadTemplateInputProviderFromConfig();
						}
					}
				}
				return _templateInputProvider;
			}
		}
		private ITemplateInputProvider _templateInputProvider;
		private readonly object _templateInputInitLock = new object();

		/// <summary>
		/// Gets the currently configured Template Signature Provider (provides a unique signature for a template's state for later comparison)
		/// </summary>
		public virtual ITemplateSignatureProvider TemplateSignatureProvider
		{
			get
			{
				if (_templateSignatureProvider == null)
				{
					lock (_templateSignatureInitLock)
					{
						if (_templateSignatureProvider == null)
						{
							_templateSignatureProvider = LoadTemplateSignatureProviderFromConfig();
						}
					}
				}
				return _templateSignatureProvider;
			}
		}
		private ITemplateSignatureProvider _templateSignatureProvider;
		private readonly object _templateSignatureInitLock = new object();

		/// <summary>
		/// Gets the currently configured Type List Provider (configures where Synthesis looks for Synthesis types to be defined and for presenters)
		/// </summary>
		public virtual ITypeListProvider TypeListProvider
		{
			get
			{
				if (_typeListProvider == null)
				{
					lock (_typeListInitLock)
					{
						if (_typeListProvider == null)
						{
							_typeListProvider = LoadTypeListProviderFromConfig();
						}
					}
				}
				return _typeListProvider;
			}
		}
		private ITypeListProvider _typeListProvider;
		private readonly object _typeListInitLock = new object();

		/// <summary>
		/// Gets the currently configured FieldNameTranslator (converts Sitecore template field names to index field names)
		/// </summary>
		public virtual FieldNameTranslator IndexFieldNameTranslator
		{
			get
			{
				if (_fieldNameTranslator == null)
				{
					lock (_fieldNameTranslatorInitLock)
					{
						if (_fieldNameTranslator == null)
						{
							_fieldNameTranslator = LoadFieldNameTranslatorFromConfig();
						}
					}
				}
				return _fieldNameTranslator;
			}
		}
		private FieldNameTranslator _fieldNameTranslator;
		private readonly object _fieldNameTranslatorInitLock = new object();

		/// <summary>
		/// Gets the currently configured InitializerProvider
		/// </summary>
		public virtual IInitializerProvider InitializerProvider
		{
			get
			{
				if (_initializerProvider == null)
				{
					lock (_initializerProviderInitLock)
					{
						if (_initializerProvider == null)
						{
							_initializerProvider = new StandardInitializerProvider(TypeListProvider);
						}
					}
				}
				return _initializerProvider;
			}
		}
		private IInitializerProvider _initializerProvider;
		private readonly object _initializerProviderInitLock = new object();

		/// <summary>
		/// Gets an instance of the Synthesis Generator pre-configured to use the current provider set
		/// </summary>
		public virtual IMetadataGenerator CreateMetadataGenerator()
		{
			return new MetadataGenerator(GeneratorParametersProvider.CreateParameters(Name), TemplateInputProvider, FieldMappingProvider, IndexFieldNameTranslator);
		}

		public virtual ITemplateCodeGenerator CreateCodeGenerator()
		{
			return new CodeDomGenerator(TemplateSignatureProvider);
		}

		/// <summary>
		/// Gets an instance of the Synthesis Synchronization Engine pre-configured to use the current provider set
		/// </summary>
		/// <returns></returns>
		public virtual SynchronizationEngine CreateSyncEngine()
		{
			return new SynchronizationEngine(TemplateSignatureProvider, CreateMetadataGenerator().GenerateMetadata(), TypeListProvider, Name);
		}

		protected virtual IFieldMappingProvider LoadFieldMappingProviderFromConfig()
		{
			// Sitecore has some serious jedi config tricks like this one
			return (IFieldMappingProvider)Factory.CreateObject("/sitecore/synthesis/providers/fieldMappingProvider", true);
		}

		protected virtual IGeneratorParametersProvider LoadGeneratorParametersProviderFromConfig()
		{
			return (IGeneratorParametersProvider)Factory.CreateObject("/sitecore/synthesis/providers/generatorParametersProvider", true);
		}

		protected virtual ITemplateInputProvider LoadTemplateInputProviderFromConfig()
		{
			return (ITemplateInputProvider)Factory.CreateObject("/sitecore/synthesis/providers/templateInputProvider", true);
		}

		protected virtual ITemplateSignatureProvider LoadTemplateSignatureProviderFromConfig()
		{
			return (ITemplateSignatureProvider)Factory.CreateObject("/sitecore/synthesis/providers/templateSignatureProvider", true);
		}

		protected virtual ITypeListProvider LoadTypeListProviderFromConfig()
		{
			return (ITypeListProvider)Factory.CreateObject("/sitecore/synthesis/providers/typeListProvider", true);
		}

		protected virtual FieldNameTranslator LoadFieldNameTranslatorFromConfig()
		{
			var indexConfiguration = Factory.GetConfigNode("/sitecore/synthesis/providers/indexConfiguration", true);

			// ReSharper disable once PossibleNullReferenceException
			var nameAttribute = indexConfiguration.Attributes["name"];

			Assert.IsNotNull(nameAttribute, "The index name was missing on the Synthesis index configuration.");

			return ContentSearchManager.GetIndex(nameAttribute.InnerText).FieldNameTranslator;
		}
	}
}
