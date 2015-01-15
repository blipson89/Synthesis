using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Synthesis.Configuration;
using Synthesis.Templates;
using Synthesis.Utility;

namespace Synthesis.Synchronization
{
	/// <summary>
	/// Engine that marshals template synchronization checks
	/// </summary>
	public class SynchronizationEngine
	{
		Dictionary<string, ModelTemplateReference> _typeLookup;
		Dictionary<string, ITemplateInfo> _templateLookup;

		readonly ITemplateSignatureProvider _signatureProvider;
		readonly ITemplateInputProvider _templateProvider;
		readonly ITypeListProvider _typeListProvider;

		public SynchronizationEngine(ITemplateSignatureProvider signatureProvider, ITemplateInputProvider templateProvider, ITypeListProvider typeListProvider)
		{
			_signatureProvider = signatureProvider;
			_templateProvider = templateProvider;
			_typeListProvider = typeListProvider;
		}

		/// <summary>
		/// Checks if a given Sitecore template has a corresponding interface. This method does NOT check any base templates this template may have for synchronization.
		/// </summary>
		public TemplateComparisonResult IsTemplateSynchronized(ITemplateInfo template)
		{
			var signature = _signatureProvider.GenerateTemplateSignature(template);
			ModelTemplateReference reference;

			if(ModelDictionary.TryGetValue(template.TemplateId.ToString(), out reference)) 
				return new TemplateComparisonResult(
					template.TemplateId.ToString(), 
					GetTemplateName(template), 
					GetModelName(reference), 
					signature, 
					reference.Metadata.VersionSignature);

			return new TemplateComparisonResult(template.TemplateId.ToString(), GetTemplateName(template), null, signature, null);
		}

		/// <summary>
		/// Checks if a given type has a corresponding Sitecore template
		/// </summary>
		public TemplateComparisonResult IsTemplateSynchronized(Type type)
		{
			return IsTemplateSynchronized(new ModelTemplateReference(type));
		}

		protected TemplateComparisonResult IsTemplateSynchronized(ModelTemplateReference reference)
		{
			ITemplateInfo template;
			if (TemplateDictionary.TryGetValue(reference.Metadata.TemplateID, out template)) 
				return new TemplateComparisonResult(
					template.TemplateId.ToString(), 
					GetTemplateName(template), 
					GetModelName(reference), 
					_signatureProvider.GenerateTemplateSignature(template), 
					reference.Metadata.VersionSignature);

			return new TemplateComparisonResult(reference.Metadata.TemplateID, null, GetModelName(reference), null, reference.Metadata.VersionSignature);
		}

		/// <summary>
		/// Checks if all templates provided by the TemplateListProvider are synchronized
		/// </summary>
		public TemplateComparisonResultCollection AreTemplatesSynchronized()
		{
			var usedTemplates = new HashSet<string>();
			var results = new List<TemplateComparisonResult>();

			TemplateComparisonResult result;
			foreach (var template in TemplateDictionary.Values)
			{
				result = IsTemplateSynchronized(template);
				results.Add(result);
				usedTemplates.Add(result.TemplateID);
			}

			foreach (var modelType in ModelDictionary)
			{
				if (!usedTemplates.Contains(modelType.Value.Metadata.TemplateID))
				{
					result = IsTemplateSynchronized(modelType.Value);
					results.Add(result);
				}
			}

			return new TemplateComparisonResultCollection(results);
		}

		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification="Doesn't make semantic sense")]
		protected static string GetTemplateName(ITemplateInfo template)
		{
			return template.FullPath.Substring("/sitecore/templates".Length);
		}

		protected static string GetModelName(ModelTemplateReference reference)
		{
			return reference.InterfaceType.FullName + ", " + reference.InterfaceType.Assembly.GetName().Name;
		}

		protected Dictionary<string, ModelTemplateReference> ModelDictionary
		{
			get
			{
				if (_typeLookup == null)
				{
					var types = _typeListProvider.CreateTypeList().WithAttribute<RepresentsSitecoreTemplateAttribute>();

					_typeLookup = new Dictionary<string, ModelTemplateReference>();

					foreach (var type in types)
					{
						if (_typeLookup.ContainsKey(type.Value.TemplateID)) throw new InvalidOperationException("The template " + type.Value.TemplateID + " has at least two representative classes " + _typeLookup[type.Value.TemplateID].InterfaceType.AssemblyQualifiedName + " and " + type.Key.AssemblyQualifiedName + ". This may indicate duplicate or corrupt models.");
						_typeLookup.Add(type.Value.TemplateID, new ModelTemplateReference(type.Key, type.Value));
					}
				}

				return _typeLookup;
			}
		}

		protected Dictionary<string, ITemplateInfo> TemplateDictionary
		{
			get
			{
				if (_templateLookup == null)
				{
					var templates = _templateProvider.CreateTemplateList().ToArray();
					_templateLookup = templates.ToDictionary(x => x.TemplateId.ToString());

					// add any dependent templates of the main template list
					foreach (var template in templates)
					{
						IEnumerable<ITemplateInfo> baseTemplates = template.AllNonstandardBaseTemplates;

						foreach (var baseTemplate in baseTemplates)
						{
							if (!_templateLookup.ContainsKey(baseTemplate.TemplateId.ToString()))
								_templateLookup.Add(baseTemplate.TemplateId.ToString(), baseTemplate);
						}
					}
				}

				return _templateLookup;
			}
		}
	}
}
