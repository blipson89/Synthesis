using System;
using Sitecore.Data;
using Sitecore.Data.Fields;
using System.Diagnostics.CodeAnalysis;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.FieldTypes
{
	/// <summary>
	/// Represents a singular item reference field type (e.g. lookup, droplink, droptree, etc) that stores its value as an ID
	/// </summary>
	public class ItemReferenceField : FieldType, IItemReferenceField
	{
		public ItemReferenceField(Lazy<Field> field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the item ID that the relationship refers to
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID", Justification = "Coherent with Sitecore convention")]
		public virtual ID TargetId
		{
			get { return ((ReferenceField)InnerField).TargetID; }
			set { SetFieldValue(value.ToString()); }
		}

		/// <summary>
		/// Gets the entity that the relationship is to. Returns null if the entity doesn't exist.
		/// </summary>
		public virtual IStandardTemplateItem Target
		{
			get
			{
				if (HasValue)
					return InnerItem.Database.GetItem(TargetId).AsStronglyTyped();

				return null;
			}
		}

		/// <summary>
		/// Checks if the relationship has a value. Does not check if the ID refers to a valid entity.
		/// </summary>
		public override bool HasValue
		{
			get
			{
				if (InnerField == null) return false;
				return TargetId != (ID)null && !TargetId.IsNull && !TargetId.IsGlobalNullId;
			}
		}

		public ReferenceField ToReferenceField()
		{
			return InnerField;
		}

		public static implicit operator ReferenceField(ItemReferenceField field)
		{
			return field.ToReferenceField();
		}
	}
}
