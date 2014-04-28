using System;
using System.Linq;
using NUnit.Framework;
using Sitecore;
using Sitecore.ContentSearch;
using Synthesis.Tests.Fixtures.ContentSearch.Data;

namespace Synthesis.Tests.Fixtures.ContentSearch
{
	/*
	 * ASSUMPTIONS OF THESE TESTS:
	 * Standard sitecore_master_index configured
	 * All items in master are allowed to be indexed
	 * The _system_ templates are relatively unmodified (used as query playground)
	 * 
	 * Yes this is kind of a hack. Sshh :)
	 */
	[TestFixture]
	[Category("Content Search Tests")]
	public class FieldTypeTests : ContentSearchTestFixture
	{
		[Test]
		public void BooleanField_SearchByValue_FindsItem_WhenBooleanTrue()
		{
			using (var context = CreateTestSearchContext())
			{
				using (new InitializerForcer(new SearchTemplateItemInitializer()))
				{
					var query = context.GetSynthesisQueryable<ISearchTemplateItem>().Where(x => x.BooleanField.Value).TakeValidDatabaseItems(10).ToArray();

					//Assert.IsTrue(query.Length > 0, "Query for boolean true items returned no results"); No results in standard db
					Assert.IsTrue(query.All(x => x.BooleanField.Value), "Query included boolean false results!");
				}
			}
		}

		[Test]
		public void BooleanField_SearchByValue_FindsItem_WhenBooleanFalse()
		{
			using (var context = CreateTestSearchContext())
			{
				using (new InitializerForcer(new SearchTemplateItemInitializer()))
				{
					var query = context.GetSynthesisQueryable<ISearchTemplateItem>().Where(x => !x.BooleanField.Value).TakeValidDatabaseItems(10).ToArray();

					Assert.IsTrue(query.Length > 0, "Query for boolean false items returned no results");
					Assert.IsTrue(query.All(x => !x.BooleanField.Value), "Query included boolean true results!");
				}
			}
		}

		[Test]
		public void MultilistField_SearchByContains_FindsItems()
		{
			using (var context = CreateTestSearchContext())
			{
				using (new InitializerForcer(new SearchTemplateItemInitializer()))
				{
					var query = context.GetSynthesisQueryable<ISearchTemplateItem>(false).Where(x => x.MultilistField.Contains(TemplateIDs.Template)).ToArray();

					Assert.IsTrue(query.Length > 0, "Query for multilist items returned no results");
					Assert.IsTrue(query.All(x => x.MultilistField.Contains(TemplateIDs.Template)), "Query included results without the specified multilist value!");
				}
			}
		}

		[Test]
		public void DateField_SearchByGreaterThan_FindsItems()
		{
			using (var context = CreateTestSearchContext())
			{
				using (new InitializerForcer(new SearchTemplateItemInitializer()))
				{
					var query = context.GetSynthesisQueryable<ISearchTemplateItem>(false).Where(x => x.Timestamp.Value > new DateTime(1971, 1, 1)).Take(10).ToArray();

					Assert.IsTrue(query.Length > 0, "Query for date items returned no results");
					Assert.IsTrue(query.All(x => x.Timestamp.Value > new DateTime(1971, 1, 1)), "Query included results without the specified date value!");
				}
			}
		}
	}
}
