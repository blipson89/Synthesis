using System.Collections.Generic;
using Synthesis.Templates;

namespace Synthesis.Generation.Model
{
	/// <summary>
	/// Stores state data about a template that's undergoing the generation process, and manages its namespaces, type names, etc
	/// </summary>
	public class ConcreteTemplateInfo
	{
		private readonly ITemplateInfo _templateInfo;
		private readonly List<FieldPropertyInfo> _fields = new List<FieldPropertyInfo>();
		private readonly List<InterfaceTemplateInfo> _implementedInterfaces = new List<InterfaceTemplateInfo>(); 

		public ConcreteTemplateInfo(ITemplateInfo templateInfo)
		{
			_templateInfo = templateInfo;
		}

		public ITemplateInfo Template { get { return _templateInfo; } }

		/// <summary>
		/// Full name of the concrete type, including relative namespace and type (no assembly)
		/// </summary>
		public string FullName { get; set; }

		/// <summary>
		/// The fields to generate.
		/// </summary>
		public IList<FieldPropertyInfo> FieldsToGenerate { get { return _fields; } }

		/// <summary>
		/// All immediate interfaces implemented by this template
		/// </summary>
		public IList<InterfaceTemplateInfo> InterfacesImplemented { get { return _implementedInterfaces; } } 

		/// <summary>
		/// Name of the concrete type
		/// </summary>
		public string TypeName
		{
			get
			{
				if (FullName.Contains(".")) return FullName.Substring(FullName.LastIndexOf('.') + 1);

				return FullName;
			}
		}

		/// <summary>
		/// Relative namespace from root for the concrete or interface
		/// </summary>
		public string RelativeNamespace
		{
			get
			{
				if (FullName.Contains(".")) return FullName.Substring(0, FullName.LastIndexOf('.'));

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
		/// Gets the full type name for the concrete type, including the complete namespace and type name, given a base namespace to append the relative namespace to
		/// </summary>
		public string GetFullTypeName(string rootNamespace)
		{
			return (GetNamespace(rootNamespace) + "." + TypeName).TrimStart('.');
		}
	}
}
