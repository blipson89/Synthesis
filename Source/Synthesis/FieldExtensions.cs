using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Fields;
using Synthesis.Configuration;
using FieldType = Synthesis.FieldTypes.FieldType;

namespace Synthesis
{
	public static class FieldExtensions
	{
		/// <summary>
		/// Converts a Sitecore Field into a specific Synthesis field equivalent
		/// </summary>
		public static TField As<TField>(this Sitecore.Data.Fields.Field field)
			where TField : FieldType
		{
			return AsStronglyTyped(field) as TField;
		}

		/// <summary>
		/// Converts a Sitecore Field into a Synthesis field equivalent
		/// </summary>
		public static FieldType AsStronglyTyped(this Sitecore.Data.Fields.Field field)
		{
			if (field == null) return null;

			var mapping = ProviderResolver.Current.FieldMappingProvider.GetFieldType(field.Type);

			var lazy = new Lazy<Field>(() => field);

			return Activator.CreateInstance(mapping, lazy, null) as FieldType;
		}
	}
}
