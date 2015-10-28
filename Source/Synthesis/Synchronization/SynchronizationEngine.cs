using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Synthesis.Configuration;
using Synthesis.Generation.Model;
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
		readonly TemplateGenerationMetadata _templateMetadata;
		readonly ITypeListProvider _typeListProvider;
		private readonly string _configurationName;

		public SynchronizationEngine(ITemplateSignatureProvider signatureProvider, TemplateGenerationMetadata templateMetadata, ITypeListProvider typeListProvider, string configurationName)
		{
			_signatureProvider = signatureProvider;
			_templateMetadata = templateMetadata;
			_typeListProvider = typeListProvider;
			_configurationName = configurationName;
		}

		/// <summary>
		/// Checks if a given Sitecore template has a corresponding interface. This method does NOT check any base templates this template may have for synchronization.
		/// </summary>
		public TemplateComparisonResult IsTemplateSynchronized(ITemplateInfo template)
		{
			var signature = _signatureProvider.GenerateTemplateSignature(template);
			ModelTemplateReference reference;

			if (ModelDictionary.TryGetValue(template.TemplateId.ToString(), out reference))
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
			if (TemplateDictionary.TryGetValue(reference.Metadata.TemplateId, out template))
				return new TemplateComparisonResult(
					template.TemplateId.ToString(),
					GetTemplateName(template),
					GetModelName(reference),
					_signatureProvider.GenerateTemplateSignature(template),
					reference.Metadata.VersionSignature);

			return new TemplateComparisonResult(reference.Metadata.TemplateId, null, GetModelName(reference), null, reference.Metadata.VersionSignature);
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
				if (!usedTemplates.Contains(modelType.Value.Metadata.TemplateId))
				{
					result = IsTemplateSynchronized(modelType.Value);
					results.Add(result);
				}
			}

			return new TemplateComparisonResultCollection(results);
		}

		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Doesn't make semantic sense")]
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
						if (_typeLookup.ContainsKey(type.Value.TemplateId)) throw new InvalidOperationException("The template " + type.Value.TemplateId + " has at least two representative classes " + _typeLookup[type.Value.TemplateId].InterfaceType.AssemblyQualifiedName + " and " + type.Key.AssemblyQualifiedName + ". This may indicate duplicate or corrupt models.");

						// ignore templates in other configurations
						if (type.Value.ConfigurationName.Equals(_configurationName))
							_typeLookup.Add(type.Value.TemplateId, new ModelTemplateReference(type.Key, type.Value));
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
					var templates = _templateMetadata.Interfaces.Select(x => x.Template).ToArray();
					_templateLookup = templates.ToDictionary(x => x.TemplateId.ToString());
					// note: metadata already contains all base templates that the generator would use
					// so we can take it at face value
				}

				return _templateLookup;
			}
		}
	}
}
