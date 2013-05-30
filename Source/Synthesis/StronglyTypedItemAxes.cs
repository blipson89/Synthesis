using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using System.Diagnostics.CodeAnalysis;

namespace Synthesis
{
	/// <summary>
	/// Version of ItemAxes that returns strongly typed entities. It also moves the Item.GetChildren() and Item.Parent in here, since they are axes really.
	/// </summary>
	public class StronglyTypedItemAxes
	{
		private ItemAxes _axes;
		private Item _item;
		public StronglyTypedItemAxes(Item item)
		{
			_item = item;
			_axes = new ItemAxes(item);
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

		[SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification="Following Sitecore Axes naming")]
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
			return _axes.GetNextSibling().AsStronglyTyped();
		}

		public bool IsAncestorOf(Item item)
		{
			return _axes.IsAncestorOf(item);
		}

		public bool IsAncestorOf(IStandardTemplateItem item)
		{
			CustomItemBase itemBase = item as CustomItemBase;

			if (itemBase == null)
				throw new ArgumentException("Can't get the item from the passed template. Make sure it derives from CustomItemBase.");

			return _axes.IsAncestorOf(itemBase.InnerItem);
		}

		public bool IsDescendantOf(Item item)
		{
			return _axes.IsDescendantOf(item);
		}

		public bool IsDescendantOf(IStandardTemplateItem item)
		{
			CustomItemBase itemBase = item as CustomItemBase;

			if (itemBase == null)
				throw new ArgumentException("Can't get the item from the passed template. Make sure it derives from CustomItemBase.");

			return _axes.IsDescendantOf(itemBase.InnerItem);
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

		public IStandardTemplateItem Parent
		{
			get { return _item.Parent.AsStronglyTyped(); }
		}
	}
}
