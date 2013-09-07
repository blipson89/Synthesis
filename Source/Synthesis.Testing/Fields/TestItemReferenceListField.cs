using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Testing.Fields
{
	/// <summary>
	/// Represents a field that has a list of item ID references (a multilist, treelist, etc)
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Collection suffix would be confusing here")]
	public class TestItemReferenceListField : TestFieldType, IItemReferenceListField
	{
		private readonly List<ID> _targetIds;
		private readonly List<IStandardTemplateItem> _standardTemplateItems;

		public TestItemReferenceListField(ID[] targetsIds = null, IStandardTemplateItem[] targets = null)
		{
			_targetIds = (targetsIds ?? new ID[0]).ToList();
			_standardTemplateItems = (targets ?? new IStandardTemplateItem[0]).ToList();
		}

		/// <summary>
		/// Gets the set of IDs that make up the relationships
		/// </summary>
		public ReadOnlyCollection<ID> TargetIds
		{
			get { return _targetIds.AsReadOnly(); }
		}

		/// <summary>
		/// Gets the items that make up the relationships
		/// </summary>
		public ReadOnlyCollection<IStandardTemplateItem> TargetItems { get { return _standardTemplateItems.AsReadOnly(); } }

		/// <summary>
		/// Checks if the relationship has one or more relations
		/// </summary>
		public override bool HasValue
		{
			get
			{
				return TargetIds.Count > 0;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)TargetIds).GetEnumerator();
		}

		public IEnumerator<ID> GetEnumerator()
		{
			return TargetIds.ToList().GetEnumerator();
		}

		public void Add(ID itemId)
		{
			_targetIds.Add(itemId);
		}

		public void Add(Item item)
		{
			Add(item.ID);
		}

		public void Add(IStandardTemplateItem item)
		{
			Add(item.Id);
		}

		public bool Remove(ID itemId)
		{
			_targetIds.Remove(itemId);

			return true;
		}

		public bool Remove(Item item)
		{
			return Remove(item.ID);
		}

		public bool Remove(IStandardTemplateItem item)
		{
			return Remove(item.Id);
		}

		public void Clear()
		{
			_targetIds.Clear();
			_standardTemplateItems.Clear();
		}

		public bool Contains(ID item)
		{
			return TargetIds.Contains(item);
		}

		public void CopyTo(ID[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public int Count
		{
			get { return TargetIds.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public MultilistField ToMultilistField()
		{
			throw new NotImplementedException("Test field types cannot access Sitecore classes.");
		}
	}
}
