using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Synthesis.Templates;
using Synthesis.Templates.Database;
using Synthesis.Utility;

namespace Synthesis.Configuration
{
	/// <summary>
	/// Configures a set of templates to generate or synchronize using the Sitecore configuration API
	/// </summary>
	/// <remarks>
	/// Initial instantiation of the object is relatively heavy, so keep a copy around for multiple calls
	/// </remarks>
	public class ConfigurationTemplateInputProvider : ITemplateInputProvider
	{
		private readonly List<string> _includedTemplateSpecs = new List<string>();
		private readonly List<string> _excludedTemplateSpecs = new List<string>();
		private readonly List<string> _excludedFieldSpecs = new List<string>();

		private HashSet<ID> _includedTemplates; // IDs of all included templates
		private HashSet<ID> _excludedTemplates; // IDs of all excluded templates
		private HashSet<ID> _excludedFields; // IDs of all excluded fields
		private List<Item> _allTemplates; // cache of all templates in the database
		private List<Item> _allFields; // cache of all fields in the database
		private List<ITemplateInfo> _templates; // cache of all templates after filtering has been done

		/// <summary>
		/// Gets all templates that match the set of include and exclude specs
		/// </summary>
		public virtual IEnumerable<ITemplateInfo> CreateTemplateList()
		{
			if (_templates == null)
			{
				RefreshSpecTargets();

				var allTemplates = GetAllTemplates();
				var acceptableTemplates = new List<TemplateItem>();
				var acceptableIds = new HashSet<ID>(_includedTemplates);
				acceptableIds.RemoveWhere(x => _excludedTemplates.Contains(x));

				foreach (var template in allTemplates)
				{
					if (acceptableIds.Contains(template.ID)) acceptableTemplates.Add(template);
				}

				_templates = acceptableTemplates.Select(x => (ITemplateInfo)new TemplateInfo(x)).ToList();
			}

			return _templates;
		}

		/// <summary>
		/// Determine if a field is not present in the excluded field set
		/// </summary>
		public bool IsFieldIncluded(ID fieldId)
		{
			if (_excludedFields == null) RefreshSpecTargets();

			// ReSharper disable PossibleNullReferenceException
			return !_excludedFields.Contains(fieldId);
			// ReSharper restore PossibleNullReferenceException
		}

		/// <summary>
		/// Clears the template cache, forcing a refresh with the latest Sitecore data.
		/// </summary>
		public void Refresh()
		{
			_templates = null;
			_allFields = null;
			_allTemplates = null;
			_includedTemplates = null;
			_excludedTemplates = null;
			_excludedFields = null;
		}

		/// <summary>
		/// Recalculates the field and template IDs that match a given spec
		/// Can be used to refresh the template list after a new template or field has been created
		/// </summary>
		protected void RefreshSpecTargets()
		{
			// calculate which templates match the include specs
			_includedTemplates = new HashSet<ID>();
			foreach (var templateSpec in _includedTemplateSpecs)
			{
				foreach (var template in ResolveTemplateSpecToItems(templateSpec))
				{
					_includedTemplates.Add(template.ID);
				}
			}

			// calculate which templates match the exclude specs
			_excludedTemplates = new HashSet<ID>();
			foreach (var templateExclusionSpec in _excludedTemplateSpecs)
			{
				foreach (var template in ResolveTemplateSpecToItems(templateExclusionSpec))
				{
					_excludedTemplates.Add(template.ID);
				}
			}

			// calculate which fields match the include specs
			_excludedFields = new HashSet<ID>();
			foreach (var fieldExclusionSpec in _excludedFieldSpecs)
			{
				foreach (var field in ResolveFieldSpecToItems(fieldExclusionSpec))
				{
					_excludedFields.Add(field.ID);
				}
			}
		}

		/// <summary>
		/// Adds a template spec to the inclusion list.
		/// </summary>
		/// <param name="templateSpec">A spec to add to the templates list (name, ID, path, or wildcard)</param>
		public void AddTemplatePath(string templateSpec)
		{
			_templates = null; // clear the template cache if it is set
			_includedTemplateSpecs.Add(templateSpec);
		}

		/// <summary>
		/// Adds a template spec to the exclusion list. Any templates to be excluded must already be on the inclusion list.
		/// </summary>
		/// <param name="templateExclusionSpec">A spec to add to the template exclusion list (name, ID, path, or wildcard)</param>
		public void AddTemplateExclusion(string templateExclusionSpec)
		{
			_templates = null; // clear the template cache if it is set
			_excludedTemplateSpecs.Add(templateExclusionSpec);
		}

		/// <summary>
		/// Adds a field spec to the field exclusion list.
		/// </summary>
		/// <param name="fieldExclusionSpec">A spec to add to the field exclusion list (name, ID, path, or wildcard, or templatespec::fieldspec using any combination of spec types)</param>
		public void AddFieldExclusion(string fieldExclusionSpec)
		{
			_templates = null; // clear the template cache if it is set
			_excludedFieldSpecs.Add(fieldExclusionSpec);
		}

		/// <summary>
		/// Takes a template spec and resolves all templates that match the spec
		/// </summary>
		private IEnumerable<Item> ResolveTemplateSpecToItems(string spec)
		{
			var items = ResolveSpecToItems(spec, GetAllTemplates());

			foreach (var item in items)
			{
				if (item == null) continue;

				if (item.TemplateID == TemplateIDs.Template) yield return item;
				else // parent folder - get descendant templates and add those
				{
					foreach (var template in item.Axes.GetDescendants().Where(x => x.TemplateID == TemplateIDs.Template))
					{
						yield return template;
					}
				}
			}
		}

		/// <summary>
		/// Takes a field spec (either bare or with an attached template spec) and resolves all fields that match the set spec
		/// </summary>
		/// <param name="spec"></param>
		/// <returns>Template Field Items that are selected by the input</returns>
		private IEnumerable<Item> ResolveFieldSpecToItems(string spec)
		{
			if (!spec.Contains("::"))
			{
				foreach (var output in ResolveSpecToItems(spec, GetAllFields()))
					yield return output;
			}
			else
			{
				string[] pieces = spec.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);

				if (pieces.Length > 2) throw new ArgumentException("Field input value " + spec + " contained more data than expected");

				var templates = ResolveSpecToItems(pieces[0], GetAllTemplates());
				var fields = ResolveSpecToItems(pieces[1], GetAllFields()).ToArray();

				foreach (var template in templates)
				{
					if (template == null) continue;

					TemplateItem templateItem = template;
					foreach (var field in fields)
					{
						if (field == null) continue;

						if (templateItem.Fields.Any(x => x.ID == field.ID)) yield return field;
					}
				}
			}
		}

		/// <summary>
		/// Takes an individual spec and resolves all items that match the set spec, given a complete list of match possibilities
		/// </summary>
		/// <returns>Items that are selected by the input (templates or fields depending on the possibilities passed in)</returns>
		private IEnumerable<Item> ResolveSpecToItems(string input, IEnumerable<Item> possibilities)
		{
			// WILDCARD: Could match any possibility. Check em all and return any match(es).
			if (input.Contains("*") || input.Contains("?"))
			{
				foreach (var possibility in possibilities)
				{
					if (MatchesWildcardSpec(possibility, input)) yield return possibility;
				}
			}

			// ID: return the ID
			else if (ID.IsID(input) && Database != null) yield return Database.GetItem(input); // normalize an ID

			// PATH: Resolve item at path, return the ID
			else if (input.StartsWith("/"))
			{
				if (Database != null)
				{
					Item target = Database.GetItem(input);
					if (target == null) throw new ArgumentException("Synthesis Config Error: The path " + input + " did not map to a valid item");
					yield return target;
				}
			}
			else
			{
				// BARE NAME: Resolve all possibilities with that name
				foreach (var possibility in possibilities)
				{
					if (possibility.Name.ToUpperInvariant() == input.ToUpperInvariant()) yield return possibility;
				}
			}
		}

		/// <summary>
		/// Checks if an item matches a wildcard spec (by name or full path)
		/// </summary>
		private static bool MatchesWildcardSpec(Item item, string spec)
		{
			if (WildcardUtility.IsWildcardMatch(item.Name, spec)) return true;
			if (WildcardUtility.IsWildcardMatch(item.Paths.FullPath, spec)) return true;

			return false;
		}

		/// <summary>
		/// Gets ALL templates that reside in the default database (master)
		/// </summary>
		private IEnumerable<Item> GetAllTemplates()
		{
			/* NOTE: there may be an issue with the GetTemplates() method here modifying the enumerable. It's only intermittent and difficult to reproduce, but
			 * the code below that causes the result of the call to be enumerated before the LINQ query on it will hopefully put a nail in the issue.
			 */
			if (_allTemplates == null)
			{
				if (Database != null)
				{
					var templates = Database.Templates.GetTemplates(LanguageManager.DefaultLanguage).ToList(); // force enumeration of result to load all

					// grab the items of all templates except the standard template (which is implied by IStandardTemplateItem)
					_allTemplates = templates.Where(x => x.Name.ToUpperInvariant() != "STANDARD TEMPLATE").Select(x => x.InnerItem).ToList();
				}
				else _allTemplates = new List<Item>();
			}

			return _allTemplates;
		}

		/// <summary>
		/// Gets ALL fields that reside in the default database (master)
		/// </summary>
		/// <remarks>
		/// Uses the link database. Agnostic of what template they belong to.
		/// </remarks>
		private IEnumerable<Item> GetAllFields()
		{
			if (Database == null) return new List<Item>(); // master db didn't exist - fail

			if (_allFields == null)
			{
				var templateFieldTemplate = new TemplateItem(Database.GetItem(TemplateIDs.TemplateField));

				var referrers = Globals.LinkDatabase.GetReferrers(templateFieldTemplate.InnerItem);

				var fields = new List<Item>();

				foreach (ItemLink link in referrers)
				{
					if (link.SourceDatabaseName == templateFieldTemplate.Database.Name)
					{
						Item item = templateFieldTemplate.Database.Items[link.SourceItemID];
						if (item != null && item.TemplateID == TemplateIDs.TemplateField)
						{
							fields.Add(item);
						}
					}
				}

				_allFields = fields;
			}

			return _allFields;
		}

		private Database _database;
		private Database Database
		{
			get
			{
				if (_database == null)
				{
					try
					{
						_database = Factory.GetDatabase("master");
					}
					catch (InvalidOperationException ex)
					{
						Log.SingleFatal("Synthesis tried to get a list of templates but couldn't because the master database wasn't defined. You should disable startup sync checks on CD instances.", ex, this);
						return null;
					}
				}

				return _database;
			}
		}
	}
}
