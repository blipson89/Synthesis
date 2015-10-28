using Sitecore.Pipelines;
using Synthesis.Configuration;

namespace Synthesis.Pipelines.Initialize
{
	/// <summary>
	/// Registers the default Synthesis configuration. If this pipeline is commented, only custom configurations will exist
	/// </summary>
	public class RegisterDefaultConfiguration
	{
		public void Process(PipelineArgs args)
		{
			ProviderResolver.RegisterConfiguration(new ConfigurationProviderConfiguration());
		}
	}
}
