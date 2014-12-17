using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Synthesis.FieldTypes.Adapters
{
	/// <summary>
	///     Version of ItemAxes that returns strongly typed entities. It also moves the Item.GetChildren() and Item.Parent in here, since they are axes really.
	/// </summary>
	public class AxesAdapter : IAxesAdapter
	{
		private readonly ItemAxes _axes;
		private readonly Item _item;

		public AxesAdapter(Item item)
		{
			_item = item;
			_axes = new ItemAxes(item);
		}

		public IStandardTemplateItem Parent
		{
			get { return _item.Parent.AsStronglyTyped(); }
		}

		public IStandardTemplateItem[] GetAncestors()
		{
			return _axes.GetAncestors().AsStronglyTypedCollection().ToArray();
		}

		public IStandardTemplateItem GetChild(ID childId)
		{
			return _axes.GetChild(childId).AsStronglyTyped();
		}

		public IStandardTemplateItem GetChild(string itemName)
		{
			return _axes.GetChild(itemName).AsStronglyTyped();
		}

		public IStandardTemplateItem GetDescendant(string name)
		{
			return _axes.GetDescendant(name).AsStronglyTyped();
		}

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Following Sitecore Axes naming")]
		public IEnumerable<IStandardTemplateItem> GetDescendants()
		{
			return _axes.GetDescendants().AsStronglyTypedCollection();
		}

		public IStandardTemplateItem GetItem(string path)
		{
			return _axes.GetItem(path).AsStronglyTyped();
		}

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Following Sitecore Axes naming")]
		public IStandardTemplateItem GetNextSibling()
		{
			return _axes.GetNextSibling().AsStronglyTyped();
		}

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Following Sitecore Axes naming")]
		public IStandardTemplateItem GetPreviousSibling()
		{
			return _axes.GetPreviousSibling().AsStronglyTyped();
		}

		public bool IsAncestorOf(IStandardTemplateItem item)
		{
			return _axes.IsAncestorOf(item.InnerItem);
		}

		public bool IsDescendantOf(IStandardTemplateItem item)
		{
			return _axes.IsDescendantOf(item.InnerItem);
		}

		public IEnumerable<IStandardTemplateItem> SelectItems(string query)
		{
			return _axes.SelectItems(query).AsStronglyTypedCollection();
		}

		public IStandardTemplateItem SelectSingleItem(string query)
		{
			return _axes.SelectSingleItem(query).AsStronglyTyped();
		}

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Following Sitecore Axes naming")]
		public IEnumerable<IStandardTemplateItem> GetChildren()
		{
			return _item.GetChildren().AsStronglyTypedCollection();
		}
	}
}