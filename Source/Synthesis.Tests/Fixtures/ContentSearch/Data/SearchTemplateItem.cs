using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using Synthesis.FieldTypes;
using Synthesis.FieldTypes.Interfaces;
using FileField = Synthesis.FieldTypes.FileField;
using ImageField = Synthesis.FieldTypes.ImageField;
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

// ReSharper disable once InconsistentNaming
		[IndexField("__bucketable")]
		public IBooleanField BooleanField
		{
			get
			{
				const string readOnlyFieldName = "__bucketable";
				return new BooleanField(new LazyField(() => InnerItem.Fields[readOnlyFieldName], "Search Template", readOnlyFieldName), GetSearchFieldValue(readOnlyFieldName));
			}
		}	
		
// ReSharper disable once InconsistentNaming
		[IndexField("_templatesimplemented")]
		public IItemReferenceListField MultilistField
		{
			get
			{
				const string templatesFieldName = "_templatesimplemented";
				return new ItemReferenceListField(new LazyField(() => InnerItem.Fields[templatesFieldName], "Search Template", templatesFieldName), GetSearchFieldValue(templatesFieldName));
			}
		}

		public IDateTimeField Timestamp
		{
			get { return new DateTimeField(new LazyField(() => InnerItem.Fields[TestFields.DATETIME], "TEST", TestFields.DATETIME), null); }
		}

		public IFileField File
		{
			get { return new FileField(new LazyField(() => InnerItem.Fields[TestFields.FILE], "TEST", TestFields.FILE), null); }
		}

		public IHyperlinkField Link
		{
			get { return new HyperlinkField(new LazyField(() => InnerItem.Fields[TestFields.HYPERLINK], "TEST", TestFields.HYPERLINK), null); }
		}

		public IImageField TerriblePicture
		{
			get { return new ImageField(new LazyField(() => InnerItem.Fields[TestFields.IMAGE], "TEST", TestFields.IMAGE), null); }
		}

		public IIntegerField DaysTillChristmas
		{
			get { return new IntegerField(new LazyField(() => InnerItem.Fields[TestFields.INTEGER], "TEST", TestFields.INTEGER), null); }
		}

	

		public INumericField AccountBalance
		{
			get { return new NumericField(new LazyField(() => InnerItem.Fields[TestFields.NUMERIC], "TEST", TestFields.NUMERIC), null); }
		}

		public IItemReferenceField RelatedFolder
		{
			get { return new ItemReferenceField(new LazyField(() => InnerItem.Fields[TestFields.SINGLE_RELATION], "TEST", TestFields.SINGLE_RELATION), null); }
		}

		public ITextField Title
		{
			get { return new TextField(new LazyField(() => InnerItem.Fields[TestFields.TEXT], "TEST", TestFields.TEXT), null); }
		}
	}
}
