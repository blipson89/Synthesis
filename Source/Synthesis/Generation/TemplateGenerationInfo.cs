using Sitecore.Data.Items;
using Synthesis.Templates;

namespace Synthesis.Generation
{
	/// <summary>
	/// Stores state data about a template that's undergoing the generation process, and manages its namespaces, type names, etc
	/// </summary>
	internal class TemplateGenerationInfo : TemplateInfo
	{
		public TemplateGenerationInfo(TemplateItem template) : base(template)
		{
		}

		/// <summary>
		/// Full name of the concrete type, including relative namespace and type (no assembly)
		/// </summary>
		public string FullName { get; set; }

		/// <summary>
		/// Full name of the interface type, including relative namespace and type (no assembly)
		/// </summary>
		public string InterfaceFullName { get; set; }

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
			else if (!string.IsNullOrEmpty(rootNamespace))
				return rootNamespace;
			else return relativeNamespace;
		}

		/// <summary>
		/// Gets the full namespace for the interface type, not including the type, given a base namespace to append the relative namespace to
		/// </summary>
		public string GetFullInterfaceTypeName(string rootNamespace)
		{
			return (GetNamespace(rootNamespace) + "." + InterfaceTypeName).TrimStart('.');
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
