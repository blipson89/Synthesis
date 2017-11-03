using Sitecore.Data.Items;

namespace Synthesis.FieldTypes.Adapters
{
	public class EditingAdapter : IEditingAdapter
	{
		private readonly ItemEditing _itemEditing;

		public EditingAdapter(ItemEditing itemEditing)
		{
			_itemEditing = itemEditing;
		}

		/// <summary>
		/// Gets a value that indicates if the item is being edited.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// <c>true</c> if this instance is editing; otherwise, <c>false</c>.
		/// 
		/// </value>
		public bool IsEditing => _itemEditing.IsEditing;

		/// <summary>
		/// Marks the beginning of an editing operation.
		/// 
		/// </summary>
		public void BeginEdit()
		{
			_itemEditing.BeginEdit();
		}

		/// <summary>
		/// Cancels the edit.
		/// 
		/// </summary>
		public void CancelEdit()
		{
			_itemEditing.CancelEdit();
		}

		/// <summary>
		/// Ends an edit operation.
		/// 
		/// </summary>
		/// 
		/// <returns>
		/// The edit.
		/// </returns>
		/// 
		/// <remarks>
		/// 
		/// <para>
		/// Editing an item without calling BeginEdit throws an exception.
		/// </para>
		/// 
		/// <para>
		/// It is usually easier and more readable to using the EditContext class instead
		///             of a BeginEdit and EndEdit pair.
		/// </para>
		/// 
		/// </remarks>
		public bool EndEdit()
		{
			return _itemEditing.EndEdit();
		}

		/// <summary>
		/// Rejects the changes.
		/// 
		/// </summary>
		public void RejectChanges()
		{
			_itemEditing.RejectChanges();
		}
	}
}
