using System;
using System.Linq;

namespace Synthesis.Generation
{
	[Serializable]
	public class GeneratorParameterException : Exception
	{
		public GeneratorParameterException() { }
		public GeneratorParameterException(string message) : base(message) { }
		public GeneratorParameterException(string message, Exception inner) : base(message, inner) { }
		protected GeneratorParameterException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
