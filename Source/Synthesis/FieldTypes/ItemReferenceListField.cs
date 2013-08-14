using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Synthesis.FieldTypes
{
	/// <summary>
	/// Represents a field that has a list of item ID references (a multilist, treelist, etc)
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Collection suffix would be confusing here")]
	public class ItemReferenceListField : FieldType, ICollection<ID>
	{
		ReadOnlyCollection<IStandardTemplateItem> _targets;

		public ItemReferenceListField(Lazy<Field> field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the set of IDs that make up the relationships
		/// </summary>
		public virtual ReadOnlyCollection<ID> TargetIds
		{
			get { return new List<ID>(((MultilistField)InnerField).TargetIDs).AsReadOnly(); }
		}

		/// <summary>
		/// Gets the items that make up the relationships
		/// </summary>
		public virtual ReadOnlyCollection<IStandardTemplateItem> TargetItems
		{
			get
			{
				if (_targets == null)
				{
					var list = new List<IStandardTemplateItem>();
					IStandardTemplateItem item;

					if (HasValue)
					{
						foreach (var target in TargetIds)
						{
							item = InnerItem.Database.GetItem(target).AsStronglyTyped();

							if (item != null)
								list.Add(item);
						}
					}

					_targets = list.AsReadOnly();
				}

				return _targets;
			}
		}

		/// <summary>
		/// Checks if the relationship has one or more relations
		/// </summary>
		public override bool HasValue
		{
			get
			{
				if (InnerField == null) return false; 
				return TargetIds.Count > 0;
			}
		}

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)TargetIds).GetEnumerator();
		}

		#endregion

		#region IEnumerable<ID> Members

		public IEnumerator<ID> GetEnumerator()
		{
			return TargetIds.ToList().GetEnumerator();
		}

		#endregion

		#region ICollection<ID> Members

		public void Add(ID itemId)
		{
			SetFieldValue(delegate
			{
				((MultilistField)InnerField).Add(itemId.ToString());
			});
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
			if (!Contains(itemId))
				return false;

			SetFieldValue(delegate
			{
				((MultilistField)InnerField).Remove(itemId.ToString());
			});

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
			SetFieldValue(string.Empty);
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

		#endregion

		public MultilistField ToMultilistField()
		{
			return InnerField;
		}

		public static implicit operator MultilistField(ItemReferenceListField field)
		{
			return field.ToMultilistField();
		}
	}
}
