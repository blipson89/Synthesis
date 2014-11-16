using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Synthesis.FieldTypes.Interfaces
{
	public interface IItemReferenceListField : ICollection<ID>, IFieldType
	{
		/// <summary>
		/// Gets the set of IDs that make up the relationships
		/// </summary>
		ReadOnlyCollection<ID> TargetIds { get; }

		/// <summary>
		/// Gets the items that make up the relationships
		/// </summary>
		ReadOnlyCollection<IStandardTemplateItem> TargetItems { get; }

		void Add(Item item);
		void Add(IStandardTemplateItem item);
		bool Remove(Item item);
		bool Remove(IStandardTemplateItem item);
		MultilistField ToMultilistField();
	}
}