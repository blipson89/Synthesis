using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sitecore.Data;
using Synthesis.Templates;

namespace Synthesis.Generation.Model
{
	/// <summary>
	/// Collects and maintains data about a set of templates undergoing generation. Handles naming, uniqueness, and other state.
	/// </summary>
	public class TemplateGenerationData
	{
		private readonly GeneratorParameters _parameters;
		private readonly List<ConcreteTemplateInfo> _templates = new List<ConcreteTemplateInfo>();
		private readonly Dictionary<ID, ConcreteTemplateInfo> _templateLookup = new Dictionary<ID, ConcreteTemplateInfo>();
		private readonly TypeNovelizer _templateNovelizer;

		private readonly List<InterfaceTemplateInfo> _interfaces = new List<InterfaceTemplateInfo>();
		private readonly Dictionary<ID, InterfaceTemplateInfo> _interfaceLookup = new Dictionary<ID, InterfaceTemplateInfo>();
		private readonly TypeNovelizer _interfaceNovelizer;

		private readonly Dictionary<ID, InterfaceTemplateInfo> _friendInterfaceLookup = new Dictionary<ID, InterfaceTemplateInfo>();

		public TemplateGenerationData(bool useRelativeNamespaces, string namespaceRoot, GeneratorParameters parameters)
		{
			_parameters = parameters;
			_templateNovelizer = new TypeNovelizer(useRelativeNamespaces, namespaceRoot);
			_interfaceNovelizer = new TypeNovelizer(useRelativeNamespaces, namespaceRoot);
		}

		/// <summary>
		/// Adds a concrete template to the state collection
		/// </summary>
		public ConcreteTemplateInfo AddConcrete(ITemplateInfo item)
		{
			var templateInfo = new ConcreteTemplateInfo(item);

			templateInfo.FullName = _templateNovelizer.GetNovelFullTypeName(item.Name, item.FullPath);

			_templateLookup.Add(item.TemplateId, templateInfo);
			_templates.Add(templateInfo);

			return templateInfo;
		}

		/// <summary>
		/// Adds a interface template to the state collection
		/// </summary>
		public InterfaceTemplateInfo AddInterface(ITemplateInfo item, string interfaceSuffix)
		{
			if (_friendInterfaceLookup.ContainsKey(item.TemplateId)) throw new InvalidOperationException("The template " + item.FullPath + " is already added as a friend interface and cannot be added again.");

			var templateInfo = new InterfaceTemplateInfo(item);

			string interfaceName = "I" + item.Name.AsIdentifier() + interfaceSuffix;
			string interfaceFullPath = item.FullPath.Substring(0, item.FullPath.LastIndexOf('/') + 1) + interfaceName;

			templateInfo.InterfaceFullName = _interfaceNovelizer.GetNovelFullTypeName(interfaceName, interfaceFullPath);

			_interfaceLookup.Add(item.TemplateId, templateInfo);
			_interfaces.Add(templateInfo);

			return templateInfo;
		}

		/// <summary>
		/// Adds a friend interface template to the state collection. Friend interfaces may be used as base interfaces for the same template locally.
		/// </summary>
		public void AddFriendInterface(InterfaceTemplateInfo item)
		{
			_friendInterfaceLookup.Add(item.Template.TemplateId, item);
		}

		/// <summary>
		/// Gets the number of concrete templates in the collection
		/// </summary>
		public int ConcreteCount { get { return _templateLookup.Count; } }

		/// <summary>
		/// Gets the number of interface templates in the collection
		/// </summary>
		public int InterfaceCount { get { return _interfaceLookup.Count; } }

		/// <summary>
		/// Determines if the state collection has an concrete template that matches the template ID
		/// </summary>
		public bool Contains(ID templateId)
		{
			return _templateLookup.ContainsKey(templateId);
		}

		/// <summary>
		/// Determines if the state collection has an interface that matches the template ID
		/// Friend interfaces will also cause a match, if present.
		/// </summary>
		public bool ContainsInterface(ID templateId)
		{
			if (_interfaceLookup.ContainsKey(templateId)) return true;

			return _friendInterfaceLookup.ContainsKey(templateId);
		}

		/// <summary>
		/// Gets a template in the collection by ID
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers", Justification = "It makes sense here to index by ID")]
		public ConcreteTemplateInfo this[ID templateId]
		{
			get { return _templateLookup[templateId]; }
		}

		/// <summary>
		/// Gets all concrete types that should be generated.
		/// </summary>
		public IEnumerable<ConcreteTemplateInfo> Templates
		{
			get { return _templates.AsReadOnly(); }
		}

		/// <summary>
		/// Gets all interfaces that should be generated. Does not include friend interfaces.
		/// </summary>
		public IEnumerable<InterfaceTemplateInfo> Interfaces
		{
			get { return _interfaces.AsReadOnly(); }
		}

		public InterfaceTemplateInfo GetInterface(ID templateId)
		{
			InterfaceTemplateInfo iface;

			if (_interfaceLookup.TryGetValue(templateId, out iface)) return iface;
			if (_friendInterfaceLookup.TryGetValue(templateId, out iface)) return iface;

			return null;
		}

		public GeneratorParameters Parameters { get { return _parameters; } }
	}
}
