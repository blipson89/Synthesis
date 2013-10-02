using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Data.Items;

namespace Synthesis.Templates
{
	/// <summary>
	/// Wraps a template item and provides some additional base-template-fu
	/// </summary>
	public class TemplateInfo
	{
		private readonly TemplateItem _template;
		public TemplateInfo(TemplateItem templateItem)
		{
			_template = templateItem;
		}

		/// <summary>
		/// Gets the template this instance is wrapping
		/// </summary>
		public TemplateItem Template { get { return _template; } }

		/// <summary>
		/// Gets immediate ancestor base templates
		/// </summary>
		public ReadOnlyCollection<TemplateInfo> BaseTemplates 
		{
			get
			{
				return _template.BaseTemplates.Select(x => new TemplateInfo(x)).ToList().AsReadOnly();
			}
		}

		/// <summary>
		/// Gets all base templates (including base templates of base templates back up to the standard template)
		/// </summary>
		public ReadOnlyCollection<TemplateInfo> AllNonstandardBaseTemplates
		{
			get
			{
				return GetRecursiveBaseTemplates(this).AsReadOnly();
			}
		}

		private List<TemplateInfo> GetRecursiveBaseTemplates(TemplateInfo parent)
		{
			var parentBases = parent.BaseTemplates.Where(x => x.Template.Name.ToUpperInvariant() != "STANDARD TEMPLATE").ToList(); // we just want NON-standard base templates
			var bases = new List<TemplateInfo>(parentBases); // we instance this collection off as it gets modified during the enumeration of parentBases (thus we couldn't enumerate it)
		
			foreach (var baseTemplate in parentBases)
			{
				bases.AddRange(GetRecursiveBaseTemplates(baseTemplate));
			}

			return bases;
		}
	}
}
