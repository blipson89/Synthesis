namespace Synthesis.FieldTypes.Adapters
{
	public interface IEditingAdapter
	{
		/// <summary>
		/// Gets a value that indicates if the item is being edited.
		/// 
		/// </summary>
		/// 
		/// <value>
		/// <c>true</c> if this instance is editing; otherwise, <c>false</c>.
		/// 
		/// </value>
		bool IsEditing { get; }

		/// <summary>
		/// Marks the beginning of an editing operation.
		/// 
		/// </summary>
		void BeginEdit();

		/// <summary>
		/// Cancels the edit.
		/// 
		/// </summary>
		void CancelEdit();

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
		bool EndEdit();

		/// <summary>
		/// Rejects the changes.
		/// 
		/// </summary>
		void RejectChanges();
	}
}