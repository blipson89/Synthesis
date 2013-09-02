using System;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Synthesis.FieldTypes;
using Synthesis.FieldTypes.Interfaces;
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
			throw new System.NotImplementedException();
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

		#region IFakeTemplateItem Members

		public IBooleanField YesOrNo
		{
			get { return new BooleanField(new Lazy<Field>(() => InnerItem.Fields["Yes or No"]), null); }
		}

		public IDateTimeField Timestamp
		{
			get { return new DateTimeField(new Lazy<Field>(() => InnerItem.Fields["Timestamp"]), null); }
		}

		public IFileField File
		{
			get { return new FileField(new Lazy<Field>(() => InnerItem.Fields["File"]), null); }
		}

		public IHyperlinkField Link
		{
			get { return new HyperlinkField(new Lazy<Field>(() => InnerItem.Fields["Link"]), null); }
		}

		public IImageField TerriblePicture
		{
			get { return new ImageField(new Lazy<Field>(() => InnerItem.Fields["Terrible Picture"]), null); }
		}

		public IIntegerField DaysTillChristmas
		{
			get { return new IntegerField(new Lazy<Field>(() => InnerItem.Fields["Days Till Christmas"]), null); }
		}

		public IItemReferenceListField RelatedFolders
		{
			get { return new ItemReferenceListField(new Lazy<Field>(() => InnerItem.Fields["Related Folders"]), null); }
		}

		public INumericField AccountBalance
		{
			get { return new NumericField(new Lazy<Field>(() => InnerItem.Fields["Account Balance"]), null); }
		}

		public IItemReferenceField RelatedFolder
		{
			get { return new ItemReferenceField(new Lazy<Field>(() => InnerItem.Fields["Related Folder"]), null); }
		}

		public ITextField Title
		{
			get { return new TextField(new Lazy<Field>(() => InnerItem.Fields["Title"]), null); }
		}

		#endregion
	}
}
