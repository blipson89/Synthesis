using System;
using System.Collections.Generic;
using System.Xml;
using Sitecore.Data.Items;
using Synthesis.FieldTypes;
using System.Diagnostics.CodeAnalysis;
using Sitecore.Diagnostics;
using Synthesis.FieldTypes.Interfaces;
using Synthesis.Templates;

namespace Synthesis.Configuration
{
	/// <summary>
	/// Provides field mappings using the Sitecore configuration API
	/// </summary>
	/// <example>
	/// Sample configuration:
	/// <fieldMappingProvider type="Synthesis.Configuration.ConfigurationFieldMappingProvider, Synthesis">
	///		<fieldMappings hint="raw:AddMapping"> <!-- raw:AddMapping maps the children of this node to cause AddMapping() to be invoked for each of them -->
	///			<map field="Checkbox" type="Synthesis.FieldTypes.BooleanField, Synthesis" />
	///		</fieldMappings>
	///	</fieldMappingProvider>
	/// </example>
	public class ConfigurationFieldMappingProvider : IFieldMappingProvider
	{
		private readonly Dictionary<string, FieldMapping> _fieldMappings = new Dictionary<string, FieldMapping>();
		private readonly Dictionary<string, TemplateMapping> _templateMappings = new Dictionary<string, TemplateMapping>();
			
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification="Sitecore convention")]
		public void AddMapping(XmlNode node)
		{
			if (node.Attributes == null || node.Attributes["field"] == null || node.Attributes["type"] == null) throw new ArgumentException("Invalid field mapping; must contain both field and type attributes");

			string fieldType = node.Attributes["field"].InnerText;
			Type type = Type.GetType(node.Attributes["type"].InnerText); // parse declared type and check that it exists

			if (type == null) throw new ArgumentException("Couldn't resolve field type " + node.Attributes["type"].InnerText);

			Type interfaceType = type;
			if (node.Attributes["interface"] != null)
			{
				interfaceType = Type.GetType(node.Attributes["interface"].InnerText);

				if (interfaceType == null) throw new ArgumentException("Couldn't resolve field interface type " + node.Attributes["interface"].InnerText);
			}

			_fieldMappings.Add(fieldType, new FieldMapping(interfaceType, type));
		}

		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification = "Sitecore convention")]
		public void AddTemplateMapping(XmlNode node)
		{
			if (node.Attributes == null || node.Attributes["template"] == null || node.Attributes["field"] == null || node.Attributes["type"] == null) throw new ArgumentException("Invalid template field mapping; must contain template, field, and type attributes");

			string templateNameOrId = node.Attributes["template"].InnerText;

			string fieldName = node.Attributes["field"].InnerText;
			Type type = Type.GetType(node.Attributes["type"].InnerText); // parse declared type and check that it exists

			if (type == null) throw new ArgumentException("Couldn't resolve field type " + node.Attributes["type"].InnerText);

			Type interfaceType = type;
			if (node.Attributes["interface"] != null)
			{
				interfaceType = Type.GetType(node.Attributes["interface"].InnerText);

				if (interfaceType == null) throw new ArgumentException("Couldn't resolve field interface type " + node.Attributes["interface"].InnerText);
			}

			if(!_templateMappings.ContainsKey(templateNameOrId)) _templateMappings.Add(templateNameOrId, new TemplateMapping());

			_templateMappings[templateNameOrId].Add(fieldName, new FieldMapping(interfaceType, type));
		}

		public FieldMapping GetFieldType(ITemplateFieldInfo templateField)
		{
			Assert.ArgumentNotNull(templateField, "templateField");

			FieldMapping fieldType = GetTemplateMapping(templateField);

			if (fieldType != null) return fieldType;

			if(_fieldMappings.TryGetValue(templateField.Type, out fieldType)) return fieldType;

			return new FieldMapping(typeof(ITextField), typeof(TextField)); // if no mapping, fall back to a text field
		}

		private FieldMapping GetTemplateMapping(ITemplateFieldInfo templateField)
		{
			Assert.ArgumentNotNull(templateField, "templateField");

			TemplateMapping templateMapping;
			FieldMapping fieldMapping;

			// template map by TID
			_templateMappings.TryGetValue(templateField.Template.TemplateId.ToString(), out templateMapping);

			// template map by full path
			if (templateMapping == null)
				_templateMappings.TryGetValue(templateField.Template.FullPath, out templateMapping);

			// template map by name
			if (templateMapping == null)
				_templateMappings.TryGetValue(templateField.Template.Name, out templateMapping);

			if (templateMapping == null) return null;

			if (templateMapping.TryGetValue(templateField.Id.ToString(), out fieldMapping))
				return fieldMapping;

			if (templateMapping.TryGetValue(templateField.Name, out fieldMapping))
				return fieldMapping;

			return null;
		}

		private class TemplateMapping : Dictionary<string, FieldMapping>
		{
			
		}
	}
}
