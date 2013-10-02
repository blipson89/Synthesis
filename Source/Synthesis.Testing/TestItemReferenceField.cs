using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Testing
{
	/// <summary>
	///     Represents a singular item reference field type (e.g. lookup, droplink, droptree, etc) that stores its value as an ID
	/// </summary>
	public class TestItemReferenceField : TestFieldType, IItemReferenceField
	{
		public TestItemReferenceField(ID reference = null, IStandardTemplateItem target = null)
		{
			TargetId = reference;
			Target = target;
		}

		/// <summary>
		///     Gets the item ID that the relationship refers to
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID", Justification = "Coherent with Sitecore convention")]
		public ID TargetId { get; set; }

		/// <summary>
		///     Gets the entity that the relationship is to. Returns null if the entity doesn't exist.
		/// </summary>
		public IStandardTemplateItem Target { get; private set; }

		/// <summary>
		///     Checks if the relationship has a value. Does not check if the ID refers to a valid entity.
		/// </summary>
		public override bool HasValue
		{
			get { return TargetId != (ID) null && !TargetId.IsNull && !TargetId.IsGlobalNullId; }
		}

		public ReferenceField ToReferenceField()
		{
			throw new NotImplementedException("Test date fields cannot return Sitecore item objects");
		}
	}
}