using System.Linq;
using Sitecore.Data;

namespace Synthesis.FieldTypes.Interfaces
{
	public interface IFieldType
	{
		/// <summary>
		/// Checks if this field has a value. Note that this is always true for a boolean field.
		/// </summary>
		bool HasValue { get; }

		/// <summary>
		/// The ID of the field instance in Sitecore, used to locate fields for Validators and other internal processes.
		/// Loads the inner field if this is a search-backed instance.
		/// </summary>
		ID Id { get; }
	}
}