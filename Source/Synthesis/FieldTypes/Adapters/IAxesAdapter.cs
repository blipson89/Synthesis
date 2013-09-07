using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Sitecore.Data;

namespace Synthesis.FieldTypes.Adapters
{
	public interface IAxesAdapter
	{
		IStandardTemplateItem Parent { get; }
		IStandardTemplateItem[] GetAncestors();
		IStandardTemplateItem GetChild(ID childId);
		IStandardTemplateItem GetChild(string itemName);
		IStandardTemplateItem GetDescendant(string name);

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Following Sitecore Axes naming")]
		IEnumerable<IStandardTemplateItem> GetDescendants();

		IStandardTemplateItem GetItem(string path);

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Following Sitecore Axes naming")]
		IStandardTemplateItem GetNextSibling();

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Following Sitecore Axes naming")]
		IStandardTemplateItem GetPreviousSibling();

		bool IsAncestorOf(IStandardTemplateItem item);
		bool IsDescendantOf(IStandardTemplateItem item);
		IEnumerable<IStandardTemplateItem> SelectItems(string query);
		IStandardTemplateItem SelectSingleItem(string query);

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Following Sitecore Axes naming")]
		IEnumerable<IStandardTemplateItem> GetChildren();
	}
}