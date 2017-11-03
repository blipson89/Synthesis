using System;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Synthesis.FieldTypes.Interfaces;
using Synthesis.Utility;

namespace Synthesis.FieldTypes
{
	public abstract class FieldType : IFieldType
	{
		private readonly LazyField _innerField;

		/// <summary>
		/// Search-based field constructor
		/// </summary>
		/// <param name="innerField">A lazy load that can be invoked if this field needs to get direct access to the underlying field (eg editing, accessing values not in the index)</param>
		/// <param name="searchValue">Raw value stored in the index for this field. If the field is not from an index, pass null.</param>
		protected FieldType(LazyField innerField, string searchValue)
		{
			Assert.IsNotNull(innerField, "Callback to load inner field was null.");

			_innerField = innerField;
			InnerSearchValue = searchValue;
		}

		public abstract bool HasValue { get; }

		/// <summary>
		/// The ID of the field instance in Sitecore, used to locate fields for Validators and other internal processes.
		/// Loads the inner field if this is a search-backed instance.
		/// </summary>
		public ID Id => InnerField.ID;

		/// <summary>
		/// The field backing the field type. Use this only if you must.
		/// Loads the inner field, obviously, if this is a search-backed instance.
		/// </summary>
		/// <remarks>Note that editing the value in InnerField may cause internal state of the field item to become outdated for some field types.</remarks>
		public Field InnerField => _innerField.Value;

		/// <summary>
		/// Gets the item this field belongs to
		/// </summary>
		protected Item InnerItem => InnerField.Item;

		/// <summary>
		/// The raw string value in the search index for a search-backed field
		/// </summary>
		protected string InnerSearchValue { get; }

		/// <summary>
		/// Checks to see if the inner field has been lazy-loaded (e.g. whether this instance is database or search-backed)
		/// </summary>
		protected bool IsFieldLoaded => _innerField.IsLoaded;

		/// <summary>
		/// Sets the field's value using a string value. Will enter edit mode if the item isn't already editing. Respects security context.
		/// </summary>
		protected void SetFieldValue(string fieldValue)
		{
			var editing = new SingleFieldEditor(InnerItem);

			InnerField.Value = fieldValue;

			editing.EnsureEndEditIfStarted();
		}

		/// <summary>
		/// Sets the field's value using an action delegate to the value. Will enter edit mode if the item isn't already editing. Respects security context.
		/// </summary>
		/// <param name="fieldSetAction">Delegate method that will set the field's values.</param>
		protected void SetFieldValue(Action fieldSetAction)
		{
			var editing = new SingleFieldEditor(InnerItem);

			fieldSetAction();

			editing.EnsureEndEditIfStarted();
		}
	}
}
