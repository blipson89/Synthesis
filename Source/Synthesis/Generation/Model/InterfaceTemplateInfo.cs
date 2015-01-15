using System.Collections.Generic;
using Synthesis.Templates;

namespace Synthesis.Generation.Model
{
	/// <summary>
	/// Stores state data about a template that's undergoing the generation process, and manages its namespaces, type names, etc
	/// </summary>
	public class InterfaceTemplateInfo
	{
		private readonly ITemplateInfo _templateInfo;
		private readonly List<FieldPropertyInfo> _fields = new List<FieldPropertyInfo>();
		private readonly List<InterfaceTemplateInfo> _implementedInterfaces = new List<InterfaceTemplateInfo>(); 

		public InterfaceTemplateInfo(ITemplateInfo templateInfo)
		{
			_templateInfo = templateInfo;
		}

		public ITemplateInfo Template { get { return _templateInfo; } }

		/// <summary>
		/// Full name of the interface type, including relative namespace and type (no assembly)
		/// </summary>
		public string InterfaceFullName { get; set; }

		/// <summary>
		/// The fields to generate.
		/// </summary>
		public IList<FieldPropertyInfo> FieldsToGenerate { get { return _fields; } }

		/// <summary>
		/// All immediate interfaces implemented by this template
		/// </summary>
		public IList<InterfaceTemplateInfo> InterfacesImplemented { get { return _implementedInterfaces; } } 

		/// <summary>
		/// Name of the interface type
		/// </summary>
		public string InterfaceTypeName
		{
			get
			{
				string interfaceFullName = InterfaceFullName;
				if (interfaceFullName.Contains(".")) return interfaceFullName.Substring(interfaceFullName.LastIndexOf('.') + 1);

				return interfaceFullName;
			}
		}

		/// <summary>
		/// Relative namespace from root for the concrete or interface
		/// </summary>
		public string RelativeNamespace
		{
			get
			{
				if (InterfaceFullName.Contains(".")) return InterfaceFullName.Substring(0, InterfaceFullName.LastIndexOf('.'));

				return string.Empty;
			}
		}

		/// <summary>
		/// Gets the full namespace for the concrete type, not including the type, given a base namespace to append the relative namespace to
		/// </summary>
		public string GetNamespace(string rootNamespace)
		{
			var relativeNamespace = RelativeNamespace;

			if (!string.IsNullOrEmpty(relativeNamespace) && !string.IsNullOrEmpty(rootNamespace))
				return rootNamespace + "." + relativeNamespace;

			if (!string.IsNullOrEmpty(rootNamespace))
				return rootNamespace;

			return relativeNamespace;
		}

		/// <summary>
		/// Gets the full namespace for the interface type, not including the type, given a base namespace to append the relative namespace to
		/// </summary>
		public string GetFullInterfaceTypeName(string rootNamespace)
		{
			return (GetNamespace(rootNamespace) + "." + InterfaceTypeName).TrimStart('.');
		}
	}
}
