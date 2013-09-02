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
		private readonly Lazy<Field> _innerField;
		private readonly string _searchValue;

		/// <summary>
		/// Search-based field constructor
		/// </summary>
		/// <param name="innerField">A lazy load that can be invoked if this field needs to get direct access to the underlying field (eg editing, accessing values not in the index)</param>
		/// <param name="searchValue">Raw value stored in the index for this field. If the field is not from an index, pass null.</param>
		protected FieldType(Lazy<Field> innerField, string searchValue)
		{
			Assert.IsNotNull(innerField, "Callback to load inner field was null.");

			_innerField = innerField;
			_searchValue = searchValue;
		}

		public abstract bool HasValue { get; }

		/// <summary>
		/// The ID of the field instance in Sitecore, used to locate fields for Validators and other internal processes.
		/// Loads the inner field if this is a search-backed instance.
		/// </summary>
		public ID Id
		{
			get
			{
				return InnerField.ID;
			}
		}

		/// <summary>
		/// The field backing the field type. Use this only if you must.
		/// Loads the inner field, obviously, if this is a search-backed instance.
		/// </summary>
		/// <remarks>Note that editing the value in InnerField may cause internal state of the field item to become outdated for some field types.</remarks>
		public Field InnerField
		{
			get { return _innerField.Value; }
		}

		/// <summary>
		/// Gets the item this field belongs to
		/// </summary>
		protected Item InnerItem
		{
			get { return InnerField.Item; }
		}

		/// <summary>
		/// The raw string value in the search index for a search-backed field
		/// </summary>
		protected string InnerSearchValue
		{
			get { return _searchValue; }
		}

		/// <summary>
		/// Sets the field's value using a string value. Will enter edit mode if the item isn't already editing. Respects security context.
		/// </summary>
		protected void SetFieldValue(string fieldValue)
		{
			using (new SingleFieldEditor(InnerItem))
			{
				InnerField.Value = fieldValue;
			}
		}

		/// <summary>
		/// Sets the field's value using an action delegate to the value. Will enter edit mode if the item isn't already editing. Respects security context.
		/// </summary>
		/// <param name="fieldSetAction">Delegate method that will set the field's values.</param>
		protected void SetFieldValue(Action fieldSetAction)
		{
			using (new SingleFieldEditor(InnerItem))
			{
				fieldSetAction();
			}
		}
	}
}
