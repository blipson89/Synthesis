using System;
using System.Linq;

namespace Synthesis.Synchronization
{
	/// <summary>
	/// A reference to a model template type and its synchronization attribute
	/// </summary>
	public class ModelTemplateReference
	{
		public ModelTemplateReference(Type type)
		{
			InterfaceType = type;
			Metadata = InterfaceType.GetCustomAttributes(typeof(RepresentsSitecoreTemplateAttribute), false).FirstOrDefault() as RepresentsSitecoreTemplateAttribute;
		}

		public ModelTemplateReference(Type type, RepresentsSitecoreTemplateAttribute attribute)
		{
			InterfaceType = type;
			Metadata = attribute;
		}

		public Type InterfaceType { get; private set; }
		public RepresentsSitecoreTemplateAttribute Metadata { get; private set; }
	}
}
