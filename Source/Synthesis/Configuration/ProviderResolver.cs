using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Synthesis.Initializers;

namespace Synthesis.Configuration
{
	public sealed class ProviderResolver
	{
		private static volatile List<IProviderConfiguration> _configurations = new List<IProviderConfiguration>();

		/// <summary>
		/// Registers a new dependency configuration with the provider resolver.
		/// </summary>
		public static void RegisterConfiguration(IProviderConfiguration configuration)
		{
			_configurations.Add(configuration);
		}

		/// <summary>
		/// Gets a specific provider configuration by name
		/// </summary>
		public static IProviderConfiguration GetConfiguration(string name)
		{
			return _configurations.FirstOrDefault(x => x.Name == name);
		}

		/// <summary>
		/// Gets all currently registered provider configurations
		/// </summary>
		public static IReadOnlyCollection<IProviderConfiguration> GetConfigurations()
		{
			return _configurations.AsReadOnly();
		}

		/// <summary>
		/// Finds an initializer class for a template ID in any configuration
		/// Note that the first configuration that has a matching template ID is returned 
		/// (so if two configurations include the same template, you may have unexpected results)
		/// </summary>
		public static ITemplateInitializer FindGlobalInitializer(ID templateId)
		{
			foreach (var configuration in _configurations)
			{
				var initializer = configuration.InitializerProvider.GetInitializer(templateId);

				if(initializer != null) return initializer;
			}

			return new StandardTemplateInitializer();
		}

		/// <summary>
		/// Finds the configuration that contains a given template ID. Returns null if none do.
		/// </summary>
		public static IProviderConfiguration FindConfigurationWithTemplate(ID templateId)
		{
			foreach (var configuration in _configurations)
			{
				var initializer = configuration.InitializerProvider.GetInitializer(templateId);

				if (initializer != null) return configuration;
			}

			return null;
		}
	}
}
