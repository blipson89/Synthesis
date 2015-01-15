using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sitecore.Data;
using Synthesis.Templates;

namespace Synthesis.Generation
{
	/// <summary>
	/// Collects and maintains data about a set of templates undergoing generation. Handles naming, uniqueness, and other state.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Serves the same purpose as a collection even if not an ICollection")]
	internal class TemplateGenerationInfoCollection : IEnumerable<TemplateGenerationInfo>
	{
		private readonly List<TemplateGenerationInfo> _templateInfos = new List<TemplateGenerationInfo>();
		private readonly Dictionary<ID, TemplateGenerationInfo> _templateLookup = new Dictionary<ID, TemplateGenerationInfo>();
		private readonly HashSet<string> _existingTemplateFullNames = new HashSet<string>();

		public TemplateGenerationInfoCollection(bool useRelativeNamespaces, string namespaceRoot)
		{
			UseRelativeNamespaces = useRelativeNamespaces;
			NamespaceRoot = namespaceRoot;
		}

		private bool UseRelativeNamespaces { get; set; }
		private string NamespaceRoot { get; set; }

		/// <summary>
		/// Adds a template to the state collection
		/// </summary>
		public TemplateGenerationInfo Add(ITemplateInfo item)
		{
			var templateInfo = new TemplateGenerationInfo(item);

			templateInfo.FullName = GetTemplateFullyQualifiedName(item);

			_templateInfos.Add(templateInfo);
			_templateLookup.Add(item.TemplateId, templateInfo);

			return templateInfo;
		}

		/// <summary>
		/// Gets the number of templates in the collection
		/// </summary>
		public int Count { get { return _templateInfos.Count; } }

		public bool Contains(ID templateId)
		{
			return _templateLookup.ContainsKey(templateId);
		}

		/// <summary>
		/// Gets a template in the collection by ID
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1043:UseIntegralOrStringArgumentForIndexers", Justification = "It makes sense here to index by ID")]
		public TemplateGenerationInfo this[ID templateId]
		{
			get { return _templateLookup[templateId]; }
		}

		/// <summary>
		/// Gets a template in the collection by index
		/// </summary>
		public TemplateGenerationInfo this[int index]
		{
			get { return _templateInfos[index]; }
		}

		/// <summary>
		/// Calculates a namespace and type name for a template that is unique among all templates in the collection
		/// </summary>
		private string GetTemplateFullyQualifiedName(ITemplateInfo template)
		{
			string name;

			if (UseRelativeNamespaces)
			{
				name = template.FullPath.Replace(NamespaceRoot, string.Empty).Trim('/').Replace('/', '.');

				var nameParts = name.Split('/');

				for (int cnt = 0; cnt < nameParts.Length; cnt++)
				{
					string namePart = nameParts[cnt];
					int v;
					if (int.TryParse(namePart.Substring(0, 1), out v))
					{
						namePart = "_" + namePart;
					}

					nameParts[cnt] = namePart;
				}

				name = string.Join(".", nameParts);

				if (name.Contains("."))
				{
					// we need to make sure the namespace and full type name are both unique
					// i.e. you could have (and this happens with the standard templates) a template called 
					// foo.bar and another called foo.bar.baz - and the foo.bar namespace for foo.bar.baz wouldn't work because a type existed called foo.bar already

					string typeName = name.Substring(name.LastIndexOf('.') + 1);
					string namespaceName = name.Substring(0, name.LastIndexOf('.')).AsNovelIdentifier(_existingTemplateFullNames);

					name = string.Concat(namespaceName, ".", typeName).AsNovelIdentifier(_existingTemplateFullNames);
				}
			}
			else name = template.Name.AsNovelIdentifier(_existingTemplateFullNames);

			_existingTemplateFullNames.Add(name);

			return name;
		}

		#region IEnumerable

		public IEnumerator<TemplateGenerationInfo> GetEnumerator()
		{
			return _templateInfos.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_templateInfos).GetEnumerator();
		}

		#endregion
	}
}
