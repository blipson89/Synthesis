using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Synthesis.Templates
{
	/// <summary>
	/// Wraps a template item and provides some additional base-template-fu
	/// </summary>
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
		public virtual IList<ITemplateFieldInfo> Fields { get { throw new NotImplementedException(); } }
		public virtual string HelpText { get { throw new NotImplementedException(); } }
		public virtual string FullPath { get { throw new NotImplementedException(); } }
		public virtual IList<ITemplateFieldInfo> OwnFields { get { throw new NotImplementedException(); } }

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
			var parentBases = parent.BaseTemplates.Where(x => x.Name.ToUpperInvariant() != "STANDARD TEMPLATE").ToList(); // we just want NON-standard base templates
			var bases = new List<ITemplateInfo>(parentBases); // we instance this collection off as it gets modified during the enumeration of parentBases (thus we couldn't enumerate it)

			foreach (var baseTemplate in parentBases)
			{
				// if the bases already have this template we've got a cycle or a duplicate inheritance across a tree
				// in which case we should ignore it.
				if (bases.Any(x => x.TemplateId.Equals(baseTemplate.TemplateId))) continue;

				bases.AddRange(GetRecursiveBaseTemplates(baseTemplate));
			}

			return bases;
		}
	}
}
