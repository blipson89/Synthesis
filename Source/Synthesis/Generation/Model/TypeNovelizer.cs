using System.Collections.Generic;

namespace Synthesis.Generation.Model
{
	/// <summary>
	/// Collects and maintains data about a set of templates undergoing generation. Handles naming, uniqueness, and other state.
	/// </summary>
	public class TypeNovelizer
	{
		private readonly bool _useRelativeNamespaces;
		private readonly string _namespaceRoot;
		private readonly HashSet<string> _existingFullNames = new HashSet<string>();

		public TypeNovelizer(bool useRelativeNamespaces, string namespaceRoot)
		{
			_useRelativeNamespaces = useRelativeNamespaces;
			_namespaceRoot = namespaceRoot;
		}

		/// <summary>
		/// Calculates a namespace and type name for a template that is unique among all templates in the collection
		/// </summary>
		public string GetNovelFullTypeName(string templateName, string fullPath)
		{
			string name;

			if (_useRelativeNamespaces)
			{
				name = fullPath.Replace(_namespaceRoot, string.Empty).Trim('/').Replace('/', '.');

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
					string namespaceName = name.Substring(0, name.LastIndexOf('.')).AsNovelIdentifier(_existingFullNames);

					name = string.Concat(namespaceName, ".", typeName).AsNovelIdentifier(_existingFullNames);
				}
			}
			else name = templateName.AsNovelIdentifier(_existingFullNames);

			_existingFullNames.Add(name);

			return name;
		}
	}
}
