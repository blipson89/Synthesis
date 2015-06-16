using Sitecore.Data.Fields;
using Synthesis.FieldTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesis.FieldTypes
{
	public class PathItemReferenceField : FieldType, IPathItemReferenceField
	{
				public PathItemReferenceField(LazyField field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the item ID that the relationship refers to
		/// </summary>
		public virtual string TargetPath
		{
			get
			{
				if (!IsFieldLoaded && InnerSearchValue != null)
				{
					return InnerSearchValue;
				}

				return ((ReferenceField)InnerField).Path;
			}
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
					return InnerItem.Database.GetItem(TargetPath).AsStronglyTyped();

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
				return !string.IsNullOrWhiteSpace(TargetPath);
			}
		}

		public ReferenceField ToReferenceField()
		{
			return InnerField;
		}
	}
}
