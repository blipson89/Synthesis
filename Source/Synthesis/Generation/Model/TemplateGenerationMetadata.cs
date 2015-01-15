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
	public class TemplateGenerationMetadata
	{
		private readonly GeneratorParameters _parameters;
		private readonly List<TemplateInfo> _templates = new List<TemplateInfo>();
		private readonly Dictionary<ID, TemplateInfo> _templateLookup = new Dictionary<ID, TemplateInfo>();
		private readonly TypeNovelizer _templateNovelizer;

		private readonly List<TemplateInfo> _interfaces = new List<TemplateInfo>();
		private readonly Dictionary<ID, TemplateInfo> _interfaceLookup = new Dictionary<ID, TemplateInfo>();
		private readonly TypeNovelizer _interfaceNovelizer;

		private readonly Dictionary<ID, TemplateInfo> _friendInterfaceLookup = new Dictionary<ID, TemplateInfo>();

		public TemplateGenerationMetadata(bool useRelativeNamespaces, string namespaceRoot, GeneratorParameters parameters)
		{
			_parameters = parameters;
			_templateNovelizer = new TypeNovelizer(useRelativeNamespaces, namespaceRoot);
			_interfaceNovelizer = new TypeNovelizer(useRelativeNamespaces, namespaceRoot);
		}

		/// <summary>
		/// Adds a concrete template to the state collection
		/// </summary>
		public TemplateInfo AddConcrete(ITemplateInfo item)
		{
			var templateInfo = new TemplateInfo(item, _parameters.ItemNamespace);

			templateInfo.FullName = _templateNovelizer.GetNovelFullTypeName(item.Name, item.FullPath);

			_templateLookup.Add(item.TemplateId, templateInfo);
			_templates.Add(templateInfo);

			return templateInfo;
		}

		/// <summary>
		/// Adds a interface template to the state collection
		/// </summary>
		public TemplateInfo AddInterface(ITemplateInfo item, string interfaceSuffix)
		{
			if (_friendInterfaceLookup.ContainsKey(item.TemplateId)) throw new InvalidOperationException("The template " + item.FullPath + " is already added as a friend interface and cannot be added again.");

			var templateInfo = new TemplateInfo(item, _parameters.InterfaceNamespace);

			string interfaceName = "I" + item.Name.AsIdentifier() + interfaceSuffix;
			string interfaceFullPath = item.FullPath.Substring(0, item.FullPath.LastIndexOf('/') + 1) + interfaceName;

			templateInfo.FullName = _interfaceNovelizer.GetNovelFullTypeName(interfaceName, interfaceFullPath);

			_interfaceLookup.Add(item.TemplateId, templateInfo);
			_interfaces.Add(templateInfo);

			return templateInfo;
		}

		/// <summary>
		/// Adds a friend interface template to the state collection. Friend interfaces may be used as base interfaces for the same template locally.
		/// </summary>
		public void AddFriendInterface(TemplateInfo item)
		{
			_friendInterfaceLookup.Add(item.Template.TemplateId, item);
		}

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
		public TemplateInfo this[ID templateId]
		{
			get { return _templateLookup[templateId]; }
		}

		/// <summary>
		/// Gets all concrete types that should be generated.
		/// </summary>
		public IReadOnlyCollection<TemplateInfo> Templates
		{
			get { return _templates.AsReadOnly(); }
		}

		/// <summary>
		/// Gets all interfaces that should be generated. Does not include friend interfaces.
		/// </summary>
		public IReadOnlyCollection<TemplateInfo> Interfaces
		{
			get { return _interfaces.AsReadOnly(); }
		}

		public TemplateInfo GetInterface(ID templateId)
		{
			TemplateInfo iface;

			if (_interfaceLookup.TryGetValue(templateId, out iface)) return iface;
			if (_friendInterfaceLookup.TryGetValue(templateId, out iface)) return iface;

			return null;
		}

		public GeneratorParameters Parameters { get { return _parameters; } }
	}
}
