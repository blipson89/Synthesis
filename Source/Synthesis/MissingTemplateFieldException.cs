using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Synthesis
{
	[Serializable]
	public class MissingTemplateFieldException : Exception
	{
		public MissingTemplateFieldException()
		{
		}

		public MissingTemplateFieldException(string message) : base(message)
		{
		}

		public MissingTemplateFieldException(string message, Exception inner) : base(message, inner)
		{
		}

		protected MissingTemplateFieldException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}
