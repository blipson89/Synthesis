using System;
using System.Collections.Generic;
using Sitecore.Configuration;
using Synthesis.Configuration;
using Synthesis.Generation.Model;
using Synthesis.Synchronization;

namespace Synthesis
{
	public static class SynthesisHelper
	{
		/// <summary>
		/// Regenerates code for all registered Synthesis configurations
		/// </summary>
		public static void RegenerateAll()
		{
			var configurations = ProviderResolver.GetConfigurations();

			ExecMetadataWithAutoFriending(configurations, (configuration, metadata) =>
			{
				configuration.CreateCodeGenerator().Generate(metadata);
			});
		}

		/// <summary>
		/// Gets the template sync status for all registered Synthesis configurations
		/// </summary>
		public static IEnumerable<KeyValuePair<IProviderConfiguration, TemplateComparisonResultCollection>> CheckSyncAll()
		{
			var configurations = ProviderResolver.GetConfigurations();

			var results = new List<KeyValuePair<IProviderConfiguration, TemplateComparisonResultCollection>>(configurations.Count);

			ExecMetadataWithAutoFriending(configurations, (configuration, metadata) =>
			{
				results.Add(new KeyValuePair<IProviderConfiguration, TemplateComparisonResultCollection>(configuration, configuration.CreateSyncEngine(metadata).AreTemplatesSynchronized()));
			});

			return results;
		}

		public static IEnumerable<KeyValuePair<IProviderConfiguration, TemplateComparisonResultCollection>> CheckSyncAllAndRegenerate()
		{
			var configurations = ProviderResolver.GetConfigurations();

			var results = new List<KeyValuePair<IProviderConfiguration, TemplateComparisonResultCollection>>(configurations.Count);

			ExecMetadataWithAutoFriending(configurations, (configuration, metadata) =>
			{
				results.Add(new KeyValuePair<IProviderConfiguration, TemplateComparisonResultCollection>(configuration, configuration.CreateSyncEngine(metadata).AreTemplatesSynchronized()));
				configuration.CreateCodeGenerator().Generate(metadata);
			});

			return results;
		}

		private static void ExecMetadataWithAutoFriending(IEnumerable<IProviderConfiguration> configurations, Action<IProviderConfiguration, TemplateGenerationMetadata> processAction)
		{
			bool autoFriending = Settings.GetBoolSetting("Synthesis.AutoFriendMetadata", true);

			var precedents = new List<TemplateGenerationMetadata>();

			foreach (var configuration in configurations)
			{
				// make sure templates are all up to date
				configuration.TemplateInputProvider.Refresh();

				var generatorConfiguration = configuration.GeneratorParametersProvider.CreateParameters(configuration.Name);

				if (autoFriending)
				{
					foreach (var precedent in precedents)
					{
						generatorConfiguration.AddFriendMetadata(precedent);
					}
				}

				var metadata = configuration.CreateMetadataGenerator(generatorConfiguration).GenerateMetadata();

				precedents.Add(metadata);

				processAction(configuration, metadata);
			}
		}
	}
}
