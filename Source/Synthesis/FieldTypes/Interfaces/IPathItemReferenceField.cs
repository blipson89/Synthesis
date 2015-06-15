using Sitecore.Data.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthesis.FieldTypes.Interfaces
{
	public interface IPathItemReferenceField : IFieldType
	{
		/// Gets the item ID that the relationship refers to
		/// </summary>
		string TargetPath { get; set; }

		/// <summary>
		/// Gets the entity that the relationship is to. Returns null if the entity doesn't exist.
		/// </summary>
		IStandardTemplateItem Target { get; }

		ReferenceField ToReferenceField();
	}
}
