using System;
using Synthesis.Generation;

namespace Synthesis.Configuration.Registration
{
	/// <summary>
	/// Wraps an inner generator parameters provider and allows mutating its resultant parameters
	/// </summary>
	public class ProxyGeneratorParametersProvider : IGeneratorParametersProvider
	{
		private readonly IGeneratorParametersProvider _innerProvider;
		private readonly Action<GeneratorParameters> _permutationAction;

		public ProxyGeneratorParametersProvider(IGeneratorParametersProvider innerProvider, Action<GeneratorParameters> permutationAction)
		{
			_innerProvider = innerProvider;
			_permutationAction = permutationAction;
		}


		public GeneratorParameters CreateParameters(string configurationName)
		{
			// we call base here so the parameters begin as a copy of what's in global config (e.g. base classes, etc)
			var parameters = _innerProvider.CreateParameters(configurationName);

			_permutationAction(parameters);

			return parameters;
		}
	}
}
