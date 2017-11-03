using System.Collections.Generic;
using Synthesis.Generation;
using Synthesis.Templates;

namespace Synthesis.Configuration.Registration
{
	public abstract class SynthesisConfigurationRegistration : ConfigurationProviderConfiguration, ISynthesisConfigurationRegistration
	{
		protected abstract string ConfigurationName { get; }

		protected abstract IEnumerable<string> IncludedTemplates { get; }

		protected abstract string NamespaceTemplatePathRoot { get; }

		protected abstract string ModelOutputFilePath { get; }

		protected abstract string RootGeneratedNamespace { get; }

		protected virtual IEnumerable<string> ExcludedTemplates
		{
			get { yield break; }
		}

		protected virtual IEnumerable<string> ExcludedFields
		{
			get { yield return "__*"; }
		}

		protected virtual void ConfigureGeneratorParameters(GeneratorParameters parameters)
		{
			parameters.InterfaceNamespace = RootGeneratedNamespace;
			parameters.InterfaceOutputPath = ModelOutputFilePath;
			parameters.ItemNamespace = RootGeneratedNamespace + ".Concrete";
			parameters.ItemOutputPath = ModelOutputFilePath;
			parameters.TemplatePathRoot = NamespaceTemplatePathRoot;
		}

		public virtual IProviderConfiguration GetConfiguration()
		{
			Name = ConfigurationName;

			return this;
		}

		protected override IGeneratorParametersProvider LoadGeneratorParametersProviderFromConfig()
		{
			// take default config's parameters and tweak them for our config
			return new ProxyGeneratorParametersProvider(base.LoadGeneratorParametersProviderFromConfig(), ConfigureGeneratorParameters);
		}

		protected override ITemplateInputProvider LoadTemplateInputProviderFromConfig()
		{
			var templates = new ConfigurationTemplateInputProvider();

			foreach (var path in IncludedTemplates)
			{
				templates.AddTemplatePath(path);
			}

			foreach (var path in ExcludedTemplates)
			{
				templates.AddTemplateExclusion(path);
			}

			foreach (var field in ExcludedFields)
			{
				templates.AddFieldExclusion(field);
			}

			return templates;
		}

		public override string Name
		{
			get { return ConfigurationName; }
			set { base.Name = value; }
		}
	}
}
