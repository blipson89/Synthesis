using System;

namespace Synthesis.FieldTypes
{
	public interface IFieldMappingProvider
	{
		Type GetFieldType(string sitecoreFieldType);
	}
}
