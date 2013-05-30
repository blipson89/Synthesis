using System;
using Sitecore.Data.Items;

namespace Synthesis.Utility
{
	/// <summary>
	/// When used with a using clause, transparently enables item editing for a single field edit transaction if edit mode is not already enabled
	/// </summary>
	internal class SingleFieldEditor : IDisposable
	{
		private bool initiatedEdit = false;
		private Item _item;

		public SingleFieldEditor(Item item)
		{
			_item = item;
			ensureEditMode();
		}

		public void Dispose()
		{
			ensureEndEdit();
		}

		/// <summary>
		/// Ends edit mode if we initiated it, otherwise leaves it alone
		/// </summary>
		private void ensureEndEdit()
		{
			if (initiatedEdit && _item.Editing.IsEditing)
				_item.Editing.EndEdit();
		}

		/// <summary>
		/// Ensures that the item is in edit mode. Does not handle any security changes.
		/// </summary>
		public void ensureEditMode()
		{
			if (!_item.Editing.IsEditing)
			{
				_item.Editing.BeginEdit();
				initiatedEdit = true;
			}
			else initiatedEdit = false;
		}
	}
}
