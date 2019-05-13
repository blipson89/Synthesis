using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Sitecore;
using Xunit;

namespace Synthesis.Tests.Fixtures.ContentSearch
{
    /*
	 * ASSUMPTIONS OF THESE TESTS:
	 * Standard sitecore_master_index configured
	 * All items in master are allowed to be indexed
	 * The _system_ templates are relatively unmodified (used as query playground)
	 */
    [Trait("Category", "FieldType Tests")]
    public class ContentSearchQueryExtensionTests : ContentSearchTestFixture
	{
        [Fact(Skip = "Need to determine how to properly mock ContentSearch")]
        public void ContentSearch_FindsSublayoutsOrWorkflowStates_WhenContainsOrIsUsed()
		{
			using (var context = CreateTestSearchContext())
			{
				var query = context.GetSynthesisQueryable<IStandardTemplateItem>()
					.ContainsOr(x => x.TemplateIds, new[] { TemplateIDs.Sublayout, TemplateIDs.WorkflowState })
					.ToArray();

				Assert.True(query.All(x => x.TemplateIds.Contains(TemplateIDs.Sublayout) || x.TemplateIds.Contains(TemplateIDs.WorkflowState)), "ContainsOr results contained unexpected template IDs");
				Assert.True(query.Any(x => x.TemplateId == TemplateIDs.Sublayout), "ContainsOr result contained no sublayout items");
				Assert.True(query.Any(x => x.TemplateId == TemplateIDs.WorkflowState), "ContainsOr result contained no workflow state items");
			}
		}


		[Fact(Skip = "Need to determine how to properly mock ContentSearch")]
		public void ContentSearch_FindsSecurityFolderOrRenderingOptions_WhenContainsOrIsUsed()
		{
			using (var context = CreateTestSearchContext())
			{
				var listOfNames = new List<string> {"Security folder", "Rendering Options"};
				var query = context.GetSynthesisQueryable<IStandardTemplateItem>()
					.ContainsOr(x => x.Name, listOfNames)
					.ToArray();

				query.Length.Should().BeGreaterOrEqualTo(2);//, "ContainsOr (string) results contained too few results");
				Assert.True(query.Any(x => x.Name == listOfNames[0]), "ContainsOr result contained no security folder item");
				Assert.True(query.Any(x => x.Name == listOfNames[1]), "ContainsOr result contained no rendering options item");
			}
		}


	}
}
