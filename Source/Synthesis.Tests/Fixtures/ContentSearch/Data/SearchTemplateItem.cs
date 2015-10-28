using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using Synthesis.FieldTypes;
using Synthesis.FieldTypes.Interfaces;
using Synthesis.Initializers;
using TextField = Synthesis.FieldTypes.TextField;

namespace Synthesis.Tests.Fixtures.ContentSearch.Data
{
	public class SearchTemplateItemInitializer : ITemplateInitializer
	{
		public IStandardTemplateItem CreateInstance(Item innerItem)
		{
			throw new NotImplementedException("This is a search test.");
		}

		public ID InitializesTemplateId
		{
			get { return ID.Null; }
		}


		public IStandardTemplateItem CreateInstanceFromSearch(IDictionary<string, string> searchFields)
		{
			return new SearchTemplateItem(searchFields);
		}
	}

	public class SearchTemplateItem : StandardTemplateItem, ISearchTemplateItem
	{
		public SearchTemplateItem(IDictionary<string, string> searchDictionary)
			: base(searchDictionary)
		{
		}

		public override ID TemplateId { get { return ItemTemplateId; } }

		public static ID ItemTemplateId { get { return TemplateIDs.StandardTemplate; } }

		[IndexField("__bucketable")]
		public IBooleanField BooleanField
		{
			get
			{
				const string readOnlyFieldName = "__bucketable";
				return new BooleanField(new LazyField(() => InnerItem.Fields[readOnlyFieldName], "Search Template", readOnlyFieldName), GetSearchFieldValue(readOnlyFieldName));
			}
		}

		[IndexField("_templatesimplemented")]
		public IItemReferenceListField MultilistField
		{
			get
			{
				const string templatesFieldName = "_templatesimplemented";
				return new ItemReferenceListField(new LazyField(() => InnerItem.Fields[templatesFieldName], "Search Template", templatesFieldName), GetSearchFieldValue(templatesFieldName));
			}
		}

		[IndexField("__smallupdateddate")]
		public IDateTimeField Timestamp
		{

			get
			{
				const string fieldName = "__smallupdateddate";
				return new DateTimeField(new LazyField(() => InnerItem.Fields[fieldName], "Search Template", fieldName), GetSearchFieldValue(fieldName));
			}
		}

		[IndexField("_template")]
		public IItemReferenceField Lookup
		{
			get
			{
				const string fieldName = "_template";
				return new ItemReferenceField(new LazyField(() => InnerItem.Fields[fieldName], "Search Template", fieldName), GetSearchFieldValue(fieldName));
			}
		}

		[IndexField("_templatename")]
		public ITextField Text
		{
			get
			{
				const string fieldName = "_templatename";
				return new TextField(new LazyField(() => InnerItem.Fields[fieldName], "Search Template", fieldName), GetSearchFieldValue(fieldName));
			}
		}
	}
}
