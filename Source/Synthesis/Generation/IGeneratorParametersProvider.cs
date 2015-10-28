
namespace Synthesis.Generation
{
	/// <summary>
	/// Provides a default set of parameters to feed to the generator. Used to allow configuration files, etc.
	/// </summary>
	public interface IGeneratorParametersProvider
	{
		GeneratorParameters CreateParameters(string configurationName);
	}
}
