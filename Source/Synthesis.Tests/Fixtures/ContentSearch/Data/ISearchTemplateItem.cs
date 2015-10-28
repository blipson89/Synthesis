using Sitecore.ContentSearch;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Tests.Fixtures.ContentSearch.Data
{
	internal interface ISearchTemplateItem : IStandardTemplateItem
	{
		[IndexField("__bucketable")]
		IBooleanField BooleanField { get; }
		[IndexField("_templatesimplemented")]
		IItemReferenceListField MultilistField { get; }
		[IndexField("__smallupdateddate")]
		IDateTimeField Timestamp { get; }
		[IndexField("_template")]
		IItemReferenceField Lookup { get; }
		[IndexField("_templatename")]
		ITextField Text { get; }
	}
}
