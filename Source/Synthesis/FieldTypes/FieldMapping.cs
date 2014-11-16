using System;

namespace Synthesis.FieldTypes
{
	/// <summary>
	/// Represents a type mapping for a Sitecore field type to a Synthesis generated field
	/// </summary>
	public class FieldMapping
	{
		public FieldMapping(Type publicFieldType, Type internalFieldType)
		{
			PublicFieldType = publicFieldType;
			InternalFieldType = internalFieldType;
		}

		/// <summary>
		/// The internal type of the field property (e.g. 'BooleanField')
		/// </summary>
		public Type InternalFieldType { get; private set; }

		/// <summary>
		/// The external type of the field property (e.g. IBooleanField)
		/// </summary>
		public Type PublicFieldType { get; private set; }
	}
}
