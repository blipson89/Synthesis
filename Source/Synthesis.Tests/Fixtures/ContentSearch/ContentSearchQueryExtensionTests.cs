using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sitecore;
using Sitecore.ContentSearch.LuceneProvider;
using Synthesis.Solr;

namespace Synthesis.Tests.Fixtures.ContentSearch
{
	/*
	 * ASSUMPTIONS OF THESE TESTS:
	 * Standard sitecore_master_index configured
	 * All items in master are allowed to be indexed
	 * The _system_ templates are relatively unmodified (used as query playground)
	 */
	[TestFixture]
	[Category("Content Search Tests")]
	public class ContentSearchQueryExtensionTests : ContentSearchTestFixture
	{
		[Test]
		public void ContentSearch_FindsSublayoutsOrWorkflowStates_WhenContainsOrIsUsed()
		{
			using (var context = CreateTestSearchContext())
			{
				var query = ResolveSynthesisQueryable(context)
					.ContainsOr(x => x.TemplateIds, new[] { TemplateIDs.Sublayout, TemplateIDs.WorkflowState })
					.ToArray();

				Assert.IsTrue(query.All(x => x.TemplateIds.Contains(TemplateIDs.Sublayout) || x.TemplateIds.Contains(TemplateIDs.WorkflowState)), "ContainsOr results contained unexpected template IDs");
				Assert.IsTrue(query.Any(x => x.TemplateId == TemplateIDs.Sublayout), "ContainsOr result contained no sublayout items");
				Assert.IsTrue(query.Any(x => x.TemplateId == TemplateIDs.WorkflowState), "ContainsOr result contained no workflow state items");
			}
		}


		[Test]
		public void ContentSearch_FindsSecurityFolderOrRenderingOptions_WhenContainsOrIsUsed()
		{
			using (var context = CreateTestSearchContext())
			{
				var listOfNames = new List<string> {"Security folder", "Rendering Options"};
				var query = ResolveSynthesisQueryable(context)
					.ContainsOr(x => x.Name, listOfNames)
					.ToArray();

				Assert.GreaterOrEqual(2, query.Length, "ContainsOr (string) results contained too few results");
				Assert.IsTrue(query.Any(x => x.Name == listOfNames[0]), "ContainsOr result contained no security folder item");
				Assert.IsTrue(query.Any(x => x.Name == listOfNames[1]), "ContainsOr result contained no rendering options item");
			}
		}

		private static IQueryable<IStandardTemplateItem> ResolveSynthesisQueryable(Sitecore.ContentSearch.IProviderSearchContext context)
		{
			if (context is LuceneSearchContext)
				return context.GetSynthesisQueryable<IStandardTemplateItem>();
			return context.GetSolrSynthesisQueryable<IStandardTemplateItem>();
		}
	}
}
