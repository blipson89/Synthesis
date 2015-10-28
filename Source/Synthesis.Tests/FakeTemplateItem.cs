using System;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;
using Synthesis.FieldTypes;
using Synthesis.FieldTypes.Interfaces;
using Synthesis.Initializers;
using FileField = Synthesis.FieldTypes.FileField;
using ImageField = Synthesis.FieldTypes.ImageField;
using TextField = Synthesis.FieldTypes.TextField;

namespace Synthesis.Tests
{
	public class FakeTemplateItemInitializer : ITemplateInitializer
	{
		public IStandardTemplateItem CreateInstance(Item innerItem)
		{
			return new FakeTemplateItem(innerItem);
		}

		public ID InitializesTemplateId
		{
			get { return ID.Null; }
		}


		public IStandardTemplateItem CreateInstanceFromSearch(IDictionary<string, string> searchFields)
		{
			throw new NotImplementedException();
		}
	}

	public class FakeTemplateItem : StandardTemplateItem, IFakeTemplateItem
	{
		public FakeTemplateItem(Item innerItem)
			: base(innerItem)
		{
		}

		public override ID TemplateId { get { return ItemTemplateId; } }

		public static ID ItemTemplateId { get { return ID.Null; } }

		public static FakeTemplateItem Create(string name, Item parent)
		{
			return parent.Add(name, new TemplateID(ItemTemplateId)).As<FakeTemplateItem>();
		}

		public IBooleanField YesOrNo
		{
			get
			{
				return new BooleanField(new LazyField(() => InnerItem.Fields[TestFields.BOOLEAN], "TEST", TestFields.BOOLEAN), null);
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

		public IItemReferenceListField RelatedFolders
		{
			get { return new ItemReferenceListField(new LazyField(() => InnerItem.Fields[TestFields.MULTIPLE_RELATION], "TEST", TestFields.MULTIPLE_RELATION), null); }
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
