using System.Diagnostics.CodeAnalysis;
using Sitecore.Data;
using Sitecore.Data.Fields;

namespace Synthesis.FieldTypes.Interfaces
{
	public interface IItemReferenceField : IFieldType
	{
		/// <summary>
		/// Gets the item ID that the relationship refers to
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID", Justification = "Coherent with Sitecore convention")]
		ID TargetId { get; set; }

		/// <summary>
		/// Gets the entity that the relationship is to. Returns null if the entity doesn't exist.
		/// </summary>
		IStandardTemplateItem Target { get; }

		ReferenceField ToReferenceField();
	}
}