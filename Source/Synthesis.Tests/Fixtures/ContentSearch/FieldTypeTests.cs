using System;
using System.Linq;
using FluentAssertions;
using Sitecore;
using Sitecore.ContentSearch.Linq;
using Synthesis.Tests.Fixtures.ContentSearch.Data;
using Xunit;

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
    [Trait("Category", "Content Search Tests")]
    public class FieldTypeTests : ContentSearchTestFixture
	{
		[Fact(Skip = "Need to determine how to properly mock ContentSearch")]
		public void BooleanField_SearchByValue_FindsItem_WhenBooleanTrue()
		{
			ExecTest(true, queryable =>
			{
				var query = queryable.Where(x => x.BooleanField.Value).TakeValidDatabaseItems(10).ToArray();

				//Assert.IsTrue(query.Length > 0, "Query for boolean true items returned no results"); No results in standard db
				query.All(x => x.BooleanField.Value).Should().BeTrue();//, "Query included boolean false results!");
			});
		}

		[Fact(Skip = "Need to determine how to properly mock ContentSearch")]
		public void BooleanField_SearchByValue_FindsItem_WhenBooleanFalse()
		{
			ExecTest(true, queryable =>
			{
				var query = queryable.Where(x => !x.BooleanField.Value).TakeValidDatabaseItems(10).ToArray();

				query.Length.Should().BeGreaterThan(0);//, "Query for boolean false items returned no results");
				query.All(x => !x.BooleanField.Value).Should().BeTrue();//, "Query included boolean true results!");
			});
		}

		[Fact(Skip = "Need to determine how to properly mock ContentSearch")]
		public void MultilistField_SearchByContains_FindsItems()
		{
			ExecTest(false, queryable =>
			{
				var query = queryable.Where(x => x.MultilistField.Contains(TemplateIDs.Template)).ToArray();

				Assert.True(query.Length > 0, "Query for multilist items returned no results");
				Assert.True(query.All(x => x.MultilistField.Contains(TemplateIDs.Template)), "Query included results without the specified multilist value!");
			});
		}

		[Fact(Skip = "Need to determine how to properly mock ContentSearch")]
		public void DateField_SearchByGreaterThan_FindsItems()
		{
			ExecTest(true, queryable =>
			{
				var query = queryable.Where(x => x.Timestamp.Value > new DateTime(1971, 1, 1)).Take(10).ToArray();

				Assert.True(query.Length > 0, "Query for date items returned no results");
				Assert.True(query.All(x => x.Timestamp.Value > new DateTime(1971, 1, 1)), "Query included results without the specified date value!");
			});
		}

		[Fact(Skip = "Need to determine how to properly mock ContentSearch")]
		public void LookupField_SearchEqual_FindsItems()
		{
			ExecTest(true, queryable =>
			{
				var query = queryable.Where(x => x.Lookup.TargetId == TemplateIDs.Sublayout).Take(10).ToArray();

				Assert.True(query.Length > 0, "Query for lookup items returned no results");
				Assert.True(query.All(x => x.Lookup.TargetId == TemplateIDs.Sublayout), "Query included results without the specified lookup value!");
			});
		}

		[Fact(Skip = "Need to determine how to properly mock ContentSearch")]
		public void TextField_SearchEqual_FindsItems()
		{
			ExecTest(false, queryable =>
			{
				// NOTE: this syntax leads you to believe the WRONG thing. The queryable == (or .Equals()) DOES NOT WORK LIKE C# EQUALS.
				// it works like the search provider's equality. At the very least it is probably case-insensitive and it may even have an analyser
				// change the effective value. Be very wary of relying on text values - especially those with multiple words.
				var query = queryable.Where(x => x.Text.RawValue == "Template").ToArray();
				Assert.True(query.Length > 0, "Query for text equal returned no results");
				Assert.True(query.All(x => x.Text.RawValue == "Template"), "Query included results without the specified text value!");
			});
		}

		[Fact(Skip = "Need to determine how to properly mock ContentSearch")]
		public void TextField_SearchContains_FindsItems()
		{
			ExecTest(false, queryable =>
			{
				// NOTE: this syntax leads you to believe the WRONG thing. The queryable Contains() DOES NOT WORK LIKE C# CONTAINS.
				// it works like the search provider's contains. At the very least it is probably case-insensitive and it may even have an analyser
				// change the effective value. Be very wary of relying on text values - especially those with multiple words.
				var query = queryable.Where(x => x.Text.RawValue.Contains("Template")).ToArray();
				Assert.True(query.Length > 0, "Query for text contains returned no results");

				// note the requirement to use IndexOf here to explicitly specify to test without case sensitivity. This check *is* a
				// standard C# IndexOf because it is not part of the search query. We can't use Contains() here because it, unlike a query Contains,
				// is case sensitive.
				Assert.True(query.All(x => x.Text.RawValue.IndexOf("Template", StringComparison.OrdinalIgnoreCase) >= 0), "Query included results without the specified text value!");
			});
		}

		[Fact(Skip = "Need to determine how to properly mock ContentSearch")]
		public void TextField_SearchStartsWith_FindsItems()
		{
			ExecTest(false, queryable =>
			{
				var query = queryable.Where(x => x.Text.RawValue.StartsWith("T")).Take(100).ToArray();
				Assert.True(query.Length > 0, "Query for text startswith returned no results");

				Assert.True(query.All(x => x.Text.RawValue.StartsWith("T", StringComparison.OrdinalIgnoreCase)), "Query included results without the specified text value!");
			});
		}

		[Fact(Skip = "Need to determine how to properly mock ContentSearch")]
		public void TextField_FacetOn()
		{
			ExecTest(false, queryable =>
			{
				var query = queryable
					.Where(x => x.Text.RawValue.StartsWith("T"))
					.Take(100)
					.FacetOn(x => x.Text.RawValue);

				var facets = query.GetFacets();

				Assert.True(facets.Categories.Count > 0, "No valid facets found");
				Assert.True(facets.Categories.First().Values.Count > 0, "No valid facet values found");

				Assert.True(facets.Categories.SelectMany(x => x.Values).All(x => x.Name.StartsWith("T", StringComparison.OrdinalIgnoreCase) || x.AggregateCount == 0), "Facets had items without the specified text value!");
			});
		}

		private void ExecTest(bool useStandardFilters, Action<IQueryable<ISearchTemplateItem>> testBody)
		{
			using (var context = CreateTestSearchContext())
			{
				using (new InitializerForcer(new SearchTemplateItemInitializer()))
				{
						testBody(context.GetSynthesisQueryable<ISearchTemplateItem>(useStandardFilters));
				}
			}
		}
	}
}
