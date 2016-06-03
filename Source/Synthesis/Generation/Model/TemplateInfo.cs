using System.Collections.Generic;
using Sitecore.Diagnostics;
using Synthesis.Templates;

namespace Synthesis.Generation.Model
{
	/// <summary>
	/// Stores state data about a template that's undergoing the generation process, and manages its namespaces, type names, etc
	/// </summary>
	public class TemplateInfo
	{
		private readonly string _rootNamespace;
		private readonly List<FieldPropertyInfo> _fields = new List<FieldPropertyInfo>();
		private readonly List<TemplateInfo> _implementedInterfaces = new List<TemplateInfo>();

		public TemplateInfo(ITemplateInfo templateInfo, string rootNamespace)
		{
			Assert.ArgumentNotNull(templateInfo, "templateInfo");
			Assert.ArgumentNotNullOrEmpty(rootNamespace, "rootNamespace");

			Template = templateInfo;
			_rootNamespace = rootNamespace;
		}

		public ITemplateInfo Template { get; }

		/// <summary>
		/// Full name of the generated type, including RELATIVE namespace and type (no assembly)
		/// You probably want TypeName or FullTypeName instead.
		/// </summary>
		public string FullName { get; set; }

		/// <summary>
		/// The fields to generate.
		/// </summary>
		public IList<FieldPropertyInfo> FieldsToGenerate => _fields;

		/// <summary>
		/// All immediate interfaces implemented by this type
		/// </summary>
		public IList<TemplateInfo> InterfacesImplemented => _implementedInterfaces;

		/// <summary>
		/// Name of the type
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
		/// Gets the full namespace for the type, not including the type name
		/// </summary>
		public string Namespace
		{
			get
			{
				var relativeNamespace = RelativeNamespace;

				if (!string.IsNullOrEmpty(relativeNamespace))
					return _rootNamespace + "." + relativeNamespace;

				return _rootNamespace;
			}
		}

		/// <summary>
		/// Gets the full type name for the type, including the complete namespace and type name
		/// </summary>
		public string TypeFullName => Namespace + "." + TypeName.TrimStart('.');

		/// <summary>
		/// Relative namespace from root for the concrete
		/// </summary>
		protected string RelativeNamespace
		{
			get
			{
				if (FullName.Contains(".")) return FullName.Substring(0, FullName.LastIndexOf('.'));

				return string.Empty;
			}
		}
	}
}
