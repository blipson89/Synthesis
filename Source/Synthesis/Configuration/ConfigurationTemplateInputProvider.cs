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
		protected readonly List<string> IncludedTemplateSpecs = new List<string>();
		protected readonly List<string> ExcludedTemplateSpecs = new List<string>();
		protected readonly List<string> ExcludedFieldSpecs = new List<string>();

		protected HashSet<ID> IncludedTemplates; // IDs of all included templates
		protected HashSet<ID> ExcludedTemplates; // IDs of all excluded templates
		protected HashSet<ID> ExcludedFields; // IDs of all excluded fields
		protected List<Item> AllTemplates; // cache of all templates in the database
		protected List<Item> AllFields; // cache of all fields in the database
		protected List<ITemplateInfo> Templates; // cache of all templates after filtering has been done

		/// <summary>
		/// Gets all templates that match the set of include and exclude specs
		/// </summary>
		public virtual IEnumerable<ITemplateInfo> CreateTemplateList()
		{
			if (Templates == null)
			{
				RefreshSpecTargets();

				var allTemplates = GetAllTemplates();
				var acceptableTemplates = new List<TemplateItem>();
				var acceptableIds = new HashSet<ID>(IncludedTemplates);
				acceptableIds.RemoveWhere(x => ExcludedTemplates.Contains(x));

				foreach (var template in allTemplates)
				{
					if (acceptableIds.Contains(template.ID)) acceptableTemplates.Add(template);
				}

				Templates = acceptableTemplates.Select(x => (ITemplateInfo)new TemplateInfo(x)).ToList();
			}

			return Templates;
		}

		/// <summary>
		/// Determine if a field is not present in the excluded field set
		/// </summary>
		public virtual bool IsFieldIncluded(ID fieldId)
		{
			if (ExcludedFields == null) RefreshSpecTargets();

			// ReSharper disable PossibleNullReferenceException
			return !ExcludedFields.Contains(fieldId);
			// ReSharper restore PossibleNullReferenceException
		}

		/// <summary>
		/// Clears the template cache, forcing a refresh with the latest Sitecore data.
		/// </summary>
		public virtual void Refresh()
		{
			Templates = null;
			AllFields = null;
			AllTemplates = null;
			IncludedTemplates = null;
			ExcludedTemplates = null;
			ExcludedFields = null;
		}

		/// <summary>
		/// Recalculates the field and template IDs that match a given spec
		/// Can be used to refresh the template list after a new template or field has been created
		/// </summary>
		protected virtual void RefreshSpecTargets()
		{
			// calculate which templates match the include specs
			IncludedTemplates = new HashSet<ID>();
			foreach (var templateSpec in IncludedTemplateSpecs)
			{
				foreach (var template in ResolveTemplateSpecToItems(templateSpec))
				{
					IncludedTemplates.Add(template.ID);
				}
			}

			// calculate which templates match the exclude specs
			ExcludedTemplates = new HashSet<ID>();
			foreach (var templateExclusionSpec in ExcludedTemplateSpecs)
			{
				foreach (var template in ResolveTemplateSpecToItems(templateExclusionSpec))
				{
					ExcludedTemplates.Add(template.ID);
				}
			}

			// calculate which fields match the include specs
			ExcludedFields = new HashSet<ID>();
			foreach (var fieldExclusionSpec in ExcludedFieldSpecs)
			{
				foreach (var field in ResolveFieldSpecToItems(fieldExclusionSpec))
				{
					ExcludedFields.Add(field.ID);
				}
			}
		}

		/// <summary>
		/// Adds a template spec to the inclusion list.
		/// </summary>
		/// <param name="templateSpec">A spec to add to the templates list (name, ID, path, or wildcard)</param>
		public virtual void AddTemplatePath(string templateSpec)
		{
			Templates = null; // clear the template cache if it is set
			IncludedTemplateSpecs.Add(templateSpec);
		}

		/// <summary>
		/// Adds a template spec to the exclusion list. Any templates to be excluded must already be on the inclusion list.
		/// </summary>
		/// <param name="templateExclusionSpec">A spec to add to the template exclusion list (name, ID, path, or wildcard)</param>
		public virtual void AddTemplateExclusion(string templateExclusionSpec)
		{
			Templates = null; // clear the template cache if it is set
			ExcludedTemplateSpecs.Add(templateExclusionSpec);
		}

		/// <summary>
		/// Adds a field spec to the field exclusion list.
		/// </summary>
		/// <param name="fieldExclusionSpec">A spec to add to the field exclusion list (name, ID, path, or wildcard, or templatespec::fieldspec using any combination of spec types)</param>
		public virtual void AddFieldExclusion(string fieldExclusionSpec)
		{
			Templates = null; // clear the template cache if it is set
			ExcludedFieldSpecs.Add(fieldExclusionSpec);
		}

		/// <summary>
		/// Takes a template spec and resolves all templates that match the spec
		/// </summary>
		protected virtual IEnumerable<Item> ResolveTemplateSpecToItems(string spec)
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
		protected virtual IEnumerable<Item> ResolveFieldSpecToItems(string spec)
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
		protected virtual IEnumerable<Item> ResolveSpecToItems(string input, IEnumerable<Item> possibilities)
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
		protected static bool MatchesWildcardSpec(Item item, string spec)
		{
			if (WildcardUtility.IsWildcardMatch(item.Name, spec)) return true;
			if (WildcardUtility.IsWildcardMatch(item.Paths.FullPath, spec)) return true;

			return false;
		}

		/// <summary>
		/// Gets ALL templates that reside in the default database (master)
		/// </summary>
		protected virtual IEnumerable<Item> GetAllTemplates()
		{
			/* NOTE: there may be an issue with the GetTemplates() method here modifying the enumerable. It's only intermittent and difficult to reproduce, but
			 * the code below that causes the result of the call to be enumerated before the LINQ query on it will hopefully put a nail in the issue.
			 */
			if (AllTemplates == null)
			{
				if (Database != null)
				{
					var templates = Database.Templates.GetTemplates(LanguageManager.DefaultLanguage).ToList(); // force enumeration of result to load all

					// grab the items of all templates except the standard template (which is implied by IStandardTemplateItem)
					AllTemplates = templates.Where(x => x.Name.ToUpperInvariant() != "STANDARD TEMPLATE").Select(x => x.InnerItem).ToList();
				}
				else AllTemplates = new List<Item>();
			}

			return AllTemplates;
		}

		/// <summary>
		/// Gets ALL fields that reside in the default database (master)
		/// </summary>
		/// <remarks>
		/// Uses the link database. Agnostic of what template they belong to.
		/// </remarks>
		protected virtual IEnumerable<Item> GetAllFields()
		{
			if (Database == null) return new List<Item>(); // master db didn't exist - fail

			if (AllFields == null)
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

				AllFields = fields;
			}

			return AllFields;
		}

		private Database _database;
		protected virtual Database Database
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
