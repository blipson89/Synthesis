﻿using System.Collections.Generic;
using System.Diagnostics;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.Diagnostics;
using Synthesis.FieldTypes;
using Synthesis.Generation.Model;
using Synthesis.Templates;
using System.Reflection;

namespace Synthesis.Generation
{
	public class MetadataGenerator : IMetadataGenerator
	{
		private readonly GeneratorParameters _parameters;
		private HashSet<string> _baseClassFields;
		private readonly IFieldMappingProvider _fieldMappingProvider;
		private readonly ITemplateInputProvider _templateInputProvider;
		private readonly FieldNameTranslator _indexFieldNameTranslator;
		const string StandardTemplate = "STANDARD TEMPLATE";

		public MetadataGenerator(GeneratorParameters parameters, ITemplateInputProvider templateProvider, IFieldMappingProvider fieldMappingProvider, FieldNameTranslator indexFieldNameTranslator)
		{
			parameters.Validate();
			_parameters = parameters;
			
			_templateInputProvider = templateProvider;
			_fieldMappingProvider = fieldMappingProvider;
			_indexFieldNameTranslator = indexFieldNameTranslator;
		}

		public TemplateGenerationMetadata GenerateMetadata()
		{
			var timer = new Stopwatch();
			timer.Start();

			// load the templates we'll be generating into a state storage collection
			var templateData = CreateTemplateData();

			foreach (var template in templateData.Templates)
			{
				HashSet<string> fieldKeys = GetBaseFieldSet(); // get fields on base type

				fieldKeys.Add(template.TypeName); // member names cannot be the same as their enclosing type so we add the type name to the fields collection

				foreach (var baseTemplate in template.Template.AllNonstandardBaseTemplates) // similarly names can't be the same as any of their base templates' names (this would cause an incompletely implemented interface)
				{
					if (templateData.Contains(baseTemplate.TemplateId))
					{
						fieldKeys.Add(templateData[baseTemplate.TemplateId].TypeName);
					}
					else fieldKeys.Add(baseTemplate.Name.AsIdentifier()); // NOTE: you could break this if you have a base template called Foo and a field called Foo that IS NOT on the Foo template (but why would you have that?)
				}

				// generate item properties
				foreach (var field in template.Template.Fields)
				{
					if (_templateInputProvider.IsFieldIncluded(field.Id)) // query the template input provider and make sure the field is included
					{
						string propertyName = field.Name.AsNovelIdentifier(fieldKeys);

						var fieldInfo = new FieldPropertyInfo(field);
						fieldInfo.FieldPropertyName = propertyName;
						if (_indexFieldNameTranslator.GetType().Name.ToLower().Contains("solr"))
							SolrFieldMappingHack(field, fieldInfo);
						else
							fieldInfo.SearchFieldName = _indexFieldNameTranslator.GetIndexFieldName(field.Name);
						fieldInfo.FieldType = _fieldMappingProvider.GetFieldType(field);

						if (fieldInfo.FieldType == null)
						{
							Log.Warn("Synthesis: Field type resolution for " + field.Template.Name + "::" + field.Name + " failed; no mapping found for field type " + field.Type, this);
							continue; // skip adding the field for generation
						}

						// record usage of the property name
						fieldKeys.Add(propertyName);

						// add the field to the metadata
						template.FieldsToGenerate.Add(fieldInfo);
					}
				}

				// generates interfaces to represent the Sitecore template inheritance hierarchy
				TemplateInfo baseInterface = GenerateInheritedInterfaces(template.Template, templateData);
				if (baseInterface != null)
					template.InterfacesImplemented.Add(baseInterface);
			}

			timer.Stop();
			Log.Info(string.Format("Synthesis: Generated metadata for {0} concrete templates and {1} interface templates in {2} ms", templateData.Templates.Count, templateData.Interfaces.Count, timer.ElapsedMilliseconds), this);

			return templateData;
		}

		private TemplateGenerationMetadata CreateTemplateData()
		{
			var templates = _templateInputProvider.CreateTemplateList();

			var templateData = new TemplateGenerationMetadata(_parameters.UseTemplatePathForNamespace, _parameters.TemplatePathRoot, _parameters);

			foreach (var friendMetadata in _parameters.GetFriendMetadata())
			{
				foreach (var iface in friendMetadata.Interfaces)
				{
					templateData.AddFriendInterface(iface);
				}
			}

			foreach (var template in templates) templateData.AddConcrete(template);
			return templateData;
		}

		/// <summary>
		/// Generates an interface for each template that the current template derives from, recursively.
		/// </summary>
		private TemplateInfo GenerateInheritedInterfaces(ITemplateInfo template, TemplateGenerationMetadata templateData)
		{
			var existingInterface = templateData.GetInterface(template.TemplateId);
			if (existingInterface != null) return existingInterface;

			var interfaceInfo = templateData.AddInterface(template, _parameters.InterfaceSuffix);

			var fieldKeys = GetBaseFieldSet();
			fieldKeys.Add(interfaceInfo.TypeName); // member names cannot be the same as their enclosing type
			fieldKeys.Add(template.Name.AsIdentifier()); // prevent any fields from being generated that would conflict with a concrete item name as well as the interface name

			// create interface properties
			foreach (var field in template.OwnFields)
			{
				if (_templateInputProvider.IsFieldIncluded(field.Id))
				{
					string propertyName = field.Name.AsNovelIdentifier(fieldKeys);

					var fieldInfo = new FieldPropertyInfo(field);
					fieldInfo.FieldPropertyName = propertyName;
					if (_indexFieldNameTranslator.GetType().Name.ToLower().Contains("solr"))
						SolrFieldMappingHack(field, fieldInfo);
					else
						fieldInfo.SearchFieldName = _indexFieldNameTranslator.GetIndexFieldName(field.Name);
					fieldInfo.FieldType = _fieldMappingProvider.GetFieldType(field);

					if (fieldInfo.FieldType == null)
					{
						Log.Warn("Synthesis: Field type resolution for " + field.Template.Name + "::" + field.Name + " failed; no mapping found for field type " + field.Type, this);
						continue; // skip adding the field for generation
					}

					// record usage of the property name
					fieldKeys.Add(propertyName);

					// add the field to the metadata
					interfaceInfo.FieldsToGenerate.Add(fieldInfo);
				}
			}

			// add base interface inheritance
			foreach (var baseTemplate in template.BaseTemplates)
			{
				if (baseTemplate.Name.ToUpperInvariant() != StandardTemplate)
				{
					// recursively generate base templates' interfaces as needed
					var baseInterface = GenerateInheritedInterfaces(baseTemplate, templateData);
					if (baseInterface != null)
						interfaceInfo.InterfacesImplemented.Add(baseInterface); // assign interface implementation
				}
			}

			return interfaceInfo;
		}
		/// <summary>
		/// Evil hack!
		/// the sitecore solr field mapper has everything we need to identify the dynamic fields, however they dont expose them publicly.
		/// This hack is to gain access to the method GetFieldConfigurationByFieldTypeName which is essentailly a mapping of sitecore type
		/// to solr type (I.E. "Rich Text" -> "{0}_t").  This method lives in the SolrFieldMap field on the SolrFieldNameTranslator class
		/// which is what we have here as the _indexFieldNameTranslator.
		/// </summary>
		/// <param name="field"></param>
		/// <param name="fieldInfo"></param>
		private void SolrFieldMappingHack(ITemplateFieldInfo field, FieldPropertyInfo fieldInfo)
		{
			var hack = _indexFieldNameTranslator.GetType().GetField("fieldMap", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_indexFieldNameTranslator);
			var hackPartTwo = hack.GetType().GetMethod("GetFieldConfigurationByFieldTypeName").Invoke(hack, new object[] { field.Type });
			if (hackPartTwo != null)
			{
				var settings = _indexFieldNameTranslator.GetType().GetField("settings", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_indexFieldNameTranslator);
				var schema = _indexFieldNameTranslator.GetType().GetField("schema", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_indexFieldNameTranslator);
				fieldInfo.SearchFieldName = hackPartTwo.GetType().GetMethod("FormatFieldName", new[] { typeof(string), typeof(Sitecore.ContentSearch.ISearchIndexSchema), typeof(string), typeof(string) }).Invoke(hackPartTwo, new object[] { field.Name, schema, null, settings.GetType().GetMethod("DefaultLanguage").Invoke(settings, null) }).ToString();
			}
			else
				fieldInfo.SearchFieldName = _indexFieldNameTranslator.GetIndexFieldName(field.Name) + "_t";
		}
		/// <summary>
		/// Gets the writeable property names defined in the item base class (normally StandardTemplateItem) so we generate unique field names for any fields that have the same names as these
		/// </summary>
		private HashSet<string> GetBaseFieldSet()
		{
			if (_baseClassFields == null)
			{
				_baseClassFields = new HashSet<string>();

				// we ignore adding the properties that already exist in the entity base class when making entities
				var baseProperties = _parameters.ItemBaseClass.GetProperties();
				foreach (var prop in baseProperties)
					_baseClassFields.Add(prop.Name);
			}

			return new HashSet<string>(_baseClassFields);
		}
	}
}
