using Sitecore.Data.Fields;

namespace Synthesis.FieldTypes.Interfaces
{
	public interface IPathItemReferenceField : IFieldType
	{
		/// <summary>
		///  Gets the item path that the relationship refers to
		/// </summary>
		string TargetPath { get; set; }

		/// <summary>
		/// Gets the entity that the relationship is to. Returns null if the entity doesn't exist.
		/// </summary>
		IStandardTemplateItem Target { get; }

		ReferenceField ToReferenceField();
	}
}
