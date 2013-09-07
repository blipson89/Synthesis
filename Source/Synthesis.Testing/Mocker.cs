using System;
using System.Collections.Specialized;
using System.Linq;
using Moq;
using Moq.AutoMock;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Globalization;
using Synthesis.FieldTypes.Adapters;
using Synthesis.FieldTypes.Interfaces;
using Synthesis.Testing.Fields;

namespace Synthesis.Testing
{
	public class Mocker
	{
		public TModel CreateMockModel<TModel>(string name, Action<Mock<TModel>> setup = null)
			where TModel : class, IStandardTemplateItem
		{
			var autoMocker = new AutoMocker();
			autoMocker.Use<IBooleanField>(new TestBooleanField(false));
			autoMocker.Use<IDateTimeField>(new TestDateTimeField(DateTime.MinValue));
			autoMocker.Use<IDictionaryField>(new TestDictionaryField(new NameValueCollection()));
			
			var mock = autoMocker.GetMock<TModel>();

			mock.SetupGet(x => x.Name).Returns(name);
			mock.SetupGet(x => x.DisplayName).Returns(name);

			mock.SetupGet(x => x.Database).Returns(() => CreateMockDatabase());
			mock.SetupGet(x => x.Axes).Returns(() => CreateMockAxes());
			mock.SetupGet(x => x.Editing).Returns(() => CreateEditingAdapter());
			mock.SetupGet(x => x.Paths).Returns(() => CreatePathAdapter());
			mock.SetupGet(x => x.Statistics).Returns(() => CreateStatisticsAdapter());

			mock.SetupGet(x => x.Id).Returns(ID.Null);

			mock.SetupGet(x => x.InnerItem).Throws(new NotImplementedException("Can't use Sitecore items on a test instance"));

			mock.SetupGet(x => x.IsLatestVersion).Returns(true);
			mock.SetupGet(x => x.Language).Returns(() => Language.Parse("en"));

			mock.SetupGet(x => x.TemplateId).Returns(ID.Null);
			mock.SetupGet(x => x.TemplateIds).Returns(new[] { ID.Null });
			mock.SetupGet(x => x.Uri).Returns(() => new ItemUri(ID.Null, "/testing/testItem", Language.Parse("en"), new Sitecore.Data.Version(1), "master"));
			mock.SetupGet(x => x.Url).Returns("/testing/testItem");

			if (setup != null) setup(mock);

			return mock.Object;
		}	

		public virtual IAxesAdapter CreateMockAxes(Action<Mock<IAxesAdapter>> setup = null)
		{
			var mock = new Mock<IAxesAdapter>();

			mock.Setup(x => x.GetAncestors()).Returns(() => new IStandardTemplateItem[0]);
			mock.Setup(x => x.GetChild(It.IsAny<ID>())).Returns<IStandardTemplateItem>(null);
			mock.Setup(x => x.GetChild(It.IsAny<string>())).Returns<IStandardTemplateItem>(null);
			mock.Setup(x => x.GetChildren()).Returns(new IStandardTemplateItem[0]);
			mock.Setup(x => x.GetDescendant(It.IsAny<string>())).Returns<IStandardTemplateItem>(null);
			mock.Setup(x => x.GetDescendants()).Returns(new IStandardTemplateItem[0]);
			mock.Setup(x => x.GetItem(It.IsAny<string>())).Returns<IStandardTemplateItem>(null);
			mock.Setup(x => x.GetNextSibling()).Returns<IStandardTemplateItem>(null);
			mock.Setup(x => x.GetPreviousSibling()).Returns<IStandardTemplateItem>(null);
			mock.Setup(x => x.IsAncestorOf(It.IsAny<IStandardTemplateItem>())).Returns(false);
			mock.Setup(x => x.IsDescendantOf(It.IsAny<IStandardTemplateItem>())).Returns(false);
			mock.SetupGet(x => x.Parent).Returns((IStandardTemplateItem)null);
			mock.Setup(x => x.SelectItems(It.IsAny<string>())).Returns(new IStandardTemplateItem[0]);
			mock.Setup(x => x.SelectSingleItem(It.IsAny<string>())).Returns<IStandardTemplateItem>(null);

			if (setup != null)
				setup(mock);

			return mock.Object;
		}

		public virtual IDatabaseAdapter CreateMockDatabase(Action<Mock<IDatabaseAdapter>> setup = null)
		{
			var mock = new Mock<IDatabaseAdapter>();

			mock.Setup(x => x.GetItem(It.IsAny<DataUri>())).Returns<IStandardTemplateItem>(null);
			mock.Setup(x => x.GetItem(It.IsAny<ID>())).Returns<IStandardTemplateItem>(null);
			mock.Setup(x => x.GetItem(It.IsAny<ID>(), It.IsAny<Language>())).Returns<IStandardTemplateItem>(null);
			mock.Setup(x => x.GetItem(It.IsAny<ID>(), It.IsAny<Language>(), It.IsAny<Sitecore.Data.Version>())).Returns<IStandardTemplateItem>(null);
			mock.Setup(x => x.GetItem(It.IsAny<string>())).Returns<IStandardTemplateItem>(null);
			mock.Setup(x => x.GetItem(It.IsAny<string>(), It.IsAny<Language>())).Returns<IStandardTemplateItem>(null);
			mock.Setup(x => x.GetItem(It.IsAny<string>(), It.IsAny<Language>(), It.IsAny<Sitecore.Data.Version>())).Returns<IStandardTemplateItem>(null);
			mock.Setup(x => x.GetLanguages()).Returns(new LanguageCollection(new[] { Language.Parse("en") }));
			mock.Setup(x => x.GetRootItem()).Returns<IStandardTemplateItem>(null);
			mock.SetupGet(x => x.Name).Returns("masterTest");
			mock.Setup(x => x.SelectItems(It.IsAny<string>())).Returns(new IStandardTemplateItem[0]);
			mock.Setup(x => x.SelectSingleItem(It.IsAny<string>())).Returns<IStandardTemplateItem>(null);

			if (setup != null)
				setup(mock);

			return mock.Object;
		}

		public virtual IPathAdapter CreatePathAdapter(Action<Mock<IPathAdapter>> setup = null)
		{
			var mock = new Mock<IPathAdapter>();

			mock.SetupGet(x => x.ContentPath).Returns("/testing/testItem");
			mock.SetupGet(x => x.FullPath).Returns("/sitecore/content/testing/testItem");
			mock.Setup(x => x.IsAncestorOf(It.IsAny<IStandardTemplateItem>())).Returns(false);
			mock.SetupGet(x => x.IsContentItem).Returns(false);
			mock.Setup(x => x.IsDescendantOf(It.IsAny<IStandardTemplateItem>())).Returns(false);
			mock.SetupGet(x => x.IsMediaItem).Returns(false);
			mock.SetupGet(x => x.MediaPath).Returns("/testing/testItem");
			mock.SetupGet(x => x.ParentPath).Returns("/testing");
			mock.SetupGet(x => x.Path).Returns("/testing/testItem");

			if (setup != null) setup(mock);

			return mock.Object;
		}

		public virtual IEditingAdapter CreateEditingAdapter(Action<Mock<IEditingAdapter>> setup = null)
		{
			var mock = new Mock<IEditingAdapter>();

			mock.Setup(x => x.BeginEdit());
			mock.Setup(x => x.CancelEdit());
			mock.Setup(x => x.EndEdit());
			mock.SetupGet(x => x.IsEditing).Returns(false);
			mock.Setup(x => x.RejectChanges());

			if (setup != null) setup(mock);

			return mock.Object;
		}

		private IStatisticsAdapter CreateStatisticsAdapter(Action<Mock<IStatisticsAdapter>> setup = null)
		{
			var mock = new Mock<IStatisticsAdapter>();

			mock.SetupGet(x => x.Created).Returns(DateTime.MinValue);
			mock.SetupGet(x => x.CreatedBy).Returns(@"sitecore\testing");
			mock.SetupGet(x => x.Revision).Returns(ID.Null.ToString);
			mock.SetupGet(x => x.Updated).Returns(DateTime.Now);
			mock.SetupGet(x => x.UpdatedBy).Returns(@"sitecore\testing");

			if (setup != null) setup(mock);

			return mock.Object;
		}
	}
}
