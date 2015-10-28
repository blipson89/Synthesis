using System;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;
using Synthesis.Configuration;
using Synthesis.Templates.Database;
using FieldType = Synthesis.FieldTypes.FieldType;

namespace Synthesis
{
	public static class FieldExtensions
	{
		/// <summary>
		/// Converts a Sitecore Field into a specific Synthesis field equivalent
		/// </summary>
		public static TField As<TField>(this Field field)
			where TField : FieldType
		{
			return AsStronglyTyped(field) as TField;
		}

		/// <summary>
		/// Converts a Sitecore Field into a Synthesis field equivalent
		/// </summary>
		public static FieldType AsStronglyTyped(this Field field)
		{
			if (field == null) return null;

			var templateField = field.Item.Template.GetField(field.ID);
			var configuration = ProviderResolver.FindConfigurationWithTemplate(field.Item.TemplateID);

			Assert.IsNotNull(configuration, "The template field was not part of any Synthesis template.");

			var mapping = configuration.FieldMappingProvider.GetFieldType(new ItemTemplateFieldInfo(templateField));

			var lazy = new Lazy<Field>(() => field);

			return Activator.CreateInstance(mapping.InternalFieldType, lazy, null) as FieldType;
		}
	}
}
