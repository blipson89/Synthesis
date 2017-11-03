using Sitecore.Data.Items;

namespace Synthesis.Utility
{
	/// <summary>
	/// When used with a using clause, transparently enables item editing for a single field edit transaction if edit mode is not already enabled
	/// </summary>
	internal class SingleFieldEditor
	{
		private bool _initiatedEdit;
		private readonly Item _item;

		public SingleFieldEditor(Item item)
		{
			_item = item;
			EnsureEditMode();
		}

		/// <summary>
		/// Ends edit mode if we initiated it, otherwise leaves it alone
		/// </summary>
		public void EnsureEndEditIfStarted()
		{
			if (_initiatedEdit && _item.Editing.IsEditing)
				_item.Editing.EndEdit();
		}

		/// <summary>
		/// Ensures that the item is in edit mode. Does not handle any security changes.
		/// </summary>
		private void EnsureEditMode()
		{
			if (!_item.Editing.IsEditing)
			{
				_item.Editing.BeginEdit();
				_initiatedEdit = true;
			}
			else _initiatedEdit = false;
		}
	}
}
