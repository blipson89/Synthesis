using System;
using System.Collections.Generic;
using System.Xml;
using Sitecore.Data.Items;
using Synthesis.FieldTypes;
using Sitecore.Data.Templates;
using System.Diagnostics.CodeAnalysis;

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
		private Dictionary<string, Type> _fieldMappings = new Dictionary<string, Type>();

		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode", Justification="Sitecore convention")]
		public void AddMapping(XmlNode node)
		{
			if (node.Attributes["field"] == null || node.Attributes["type"] == null) throw new ArgumentException("Invalid field mapping; must contain both field and type attributes");

			string fieldName = node.Attributes["field"].InnerText;
			Type type = Type.GetType(node.Attributes["type"].InnerText); // parse declared type and check that it exists

			if (type == null) throw new ArgumentException("Couldn't resolve field type " + node.Attributes["type"].InnerText);

			_fieldMappings.Add(fieldName, type);
		}

		public Type GetFieldType(string sitecoreFieldType)
		{
			Type fieldType;
			
			if(_fieldMappings.TryGetValue(sitecoreFieldType, out fieldType)) return fieldType;

			return typeof(TextField); // if no mapping, fall back to a text field
		}
	}
}
