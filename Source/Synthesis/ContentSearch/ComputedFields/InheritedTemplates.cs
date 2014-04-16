using System.Collections.Generic;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Synthesis.ContentSearch.ComputedFields
{
	/// <summary>
	/// A computed field that correctly recurses all templates, unlike the default.
	/// 
	/// Code is courtesy of http://mikael.com/2013/05/sitecore-7-query-items-that-inherits-a-template/
	/// 
	/// Note that the default Sitecore.ContentSearch.ComputedFields.AllTemplates field does NOT do the same thing
	/// as this implementation. That one will get all _immediate_ base templates, whereas this one is
	/// recursive and gets ALL base templates, even of grandparent and above templates.
	/// </summary>
	public class InheritedTemplates : IComputedIndexField
	{
		public string FieldName { get; set; }

		public string ReturnType { get; set; }

		public object ComputeFieldValue(IIndexable indexable)
		{
			return GetAllTemplates(indexable as SitecoreIndexableItem);
		}

		private static List<string> GetAllTemplates(Item item)
		{
			Assert.ArgumentNotNull(item, "item");
			Assert.IsNotNull(item.Template, "Item template not found.");
			var list = new List<string> { IdHelper.NormalizeGuid(item.TemplateID) };
			RecurseTemplates(list, item.Template);
			return list;
		}

		private static void RecurseTemplates(ICollection<string> list, TemplateItem template)
		{
			foreach (var baseTemplateItem in template.BaseTemplates)
			{
				list.Add(IdHelper.NormalizeGuid(baseTemplateItem.ID));
				if (baseTemplateItem.ID != TemplateIDs.StandardTemplate)
					RecurseTemplates(list, baseTemplateItem);
			}
		}
	}
}
