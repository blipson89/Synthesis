using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Synthesis.Templates.Database
{
	/// <summary>
	/// Wraps a template item and provides some additional base-template-fu
	/// </summary>
	[DebuggerDisplay("{Name}")]
	public class TemplateInfo : ITemplateInfo
	{
		private readonly TemplateItem _template;
		public TemplateInfo(TemplateItem templateItem)
		{
			_template = templateItem;
		}

		protected TemplateInfo()
		{

		}

		public virtual ID TemplateId { get { return _template.ID; } }
		public virtual string Name { get { return _template.Name; } }
		public virtual string HelpText { get { return _template.InnerItem.Help.Text; } }
		public virtual string FullPath { get { return _template.InnerItem.Paths.FullPath; } }

		private List<ITemplateFieldInfo> _fields;
		public virtual IList<ITemplateFieldInfo> Fields
		{
			get
			{
				if (_fields == null)
				{
					_fields = _template.Fields.Select(x => (ITemplateFieldInfo)new ItemTemplateFieldInfo(x, this)).ToList();
				}

				return _fields.AsReadOnly();
			}
		}

		private List<ITemplateFieldInfo> _ownFields;
		public virtual IList<ITemplateFieldInfo> OwnFields
		{
			get
			{
				if (_ownFields == null)
				{
					_ownFields = _template.OwnFields.Select(x => (ITemplateFieldInfo)new ItemTemplateFieldInfo(x, this)).ToList();
				}

				return _ownFields.AsReadOnly();
			}
		}

		/// <summary>
		/// Gets immediate ancestor base templates
		/// </summary>
		public virtual ReadOnlyCollection<ITemplateInfo> BaseTemplates
		{
			get
			{
				return _template.BaseTemplates.Select(x => new TemplateInfo(x)).ToList<ITemplateInfo>().AsReadOnly();
			}
		}

		/// <summary>
		/// Gets all base templates (including base templates of base templates back up to the standard template)
		/// </summary>
		public virtual ReadOnlyCollection<ITemplateInfo> AllNonstandardBaseTemplates
		{
			get
			{
				return GetRecursiveBaseTemplates(this).AsReadOnly();
			}
		}

		private List<ITemplateInfo> GetRecursiveBaseTemplates(ITemplateInfo parent)
		{
			// let's keep it our little secret that this is an iterative method, not recursive ;)

			var parentBases = new Queue<ITemplateInfo>(parent.BaseTemplates.Where(x => x.Name.ToUpperInvariant() != "STANDARD TEMPLATE")); // we just want NON-standard base templates
			var bases = new Dictionary<ID, ITemplateInfo>();
			while (parentBases.Count > 0)
			{
				var currentBase = parentBases.Dequeue();
				
				// already processed this template; skip it (e.g. a template cycle), or if it's the parent template
				if (bases.ContainsKey(currentBase.TemplateId) || currentBase.TemplateId.Equals(parent.TemplateId)) continue;

				// add grandparent base templates to processing queue
				var newBases = currentBase.BaseTemplates.Where(x => x.Name.ToUpperInvariant() != "STANDARD TEMPLATE");
				foreach(var newBase in newBases) parentBases.Enqueue(newBase);

				// add parent base template to bases
				bases.Add(currentBase.TemplateId, currentBase);
			}
			
			return bases.Values.ToList();
		}
	}
}
