namespace Synthesis.Configuration.Registration
{
	/// <summary>
	/// Registers the default configuration.
	/// To register this, enable registering configurations for the Synthesis assembly (which Synthesis does by default)
	/// using the initialize pipeline and SynthesisConfigRegistrar processor.
	/// </summary>
	public class DefaultConfigurationRegistration : ISynthesisConfigurationRegistration
	{
		public IProviderConfiguration GetConfiguration()
		{
			return new ConfigurationProviderConfiguration();
		}
	}
}
