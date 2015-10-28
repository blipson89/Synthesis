using System;
using System.Linq;
using NUnit.Framework;
using Sitecore.Data.Items;
using Synthesis.Configuration;
using Synthesis.Tests.Utility;

namespace Synthesis.Tests.Fixtures.Configuration
{
	[TestFixture]
	[Category("Configuration Tests")]
	public class TemplateInputProviderTests
	{
		private Item AliceTemplate; // the alice template is a parent template with fields "Hello"
		private Item AliceField;
		private Item BobTemplate; // the bob template is a parent template with fields "Hi"
		private Item BobField;
		private Item AlicesonTemplate; // the aliceson template is  child template that inherits from alice and bob with fields "Child"
		private Item AlicesonField;
		private Item FlowerChildTemplate; // the flowerchild template inherits from the allison template with fields "Grandchild"
		private Item FlowerChildField;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			TemplateCreateUtility.CleanUpTestTemplatesFolder(); // make sure we only have our templates here so counts are accurate

			AliceTemplate = TemplateCreateUtility.CreateTestTemplate("Alice" + Guid.NewGuid().ToString("N"));
			Item section = AliceTemplate.CreateTemplateSection("Test");
			AliceField = section.CreateTemplateField("Hello" + Guid.NewGuid().ToString("N"), TemplateFieldTypes.TEXT_FIELD);

			BobTemplate = TemplateCreateUtility.CreateTestTemplate("Bob" + Guid.NewGuid().ToString("N"));
			section = BobTemplate.CreateTemplateSection("Test");
			BobField = section.CreateTemplateField("Hi" + Guid.NewGuid().ToString("N"), TemplateFieldTypes.TEXT_FIELD);

			AlicesonTemplate = TemplateCreateUtility.CreateTestTemplate("Aliceson" + Guid.NewGuid().ToString("N"));
			AlicesonTemplate.SetTemplateInheritance(AliceTemplate, BobTemplate);
			section = AlicesonTemplate.CreateTemplateSection("Test");
			AlicesonField = section.CreateTemplateField("Child" + Guid.NewGuid().ToString("N"), TemplateFieldTypes.TEXT_FIELD);

			FlowerChildTemplate = TemplateCreateUtility.CreateTestTemplate("FlowerChild" + Guid.NewGuid().ToString("N"));
			FlowerChildTemplate.SetTemplateInheritance(AlicesonTemplate);
			section = FlowerChildTemplate.CreateTemplateSection("Test");
			FlowerChildField = section.CreateTemplateField("Grandchild" + Guid.NewGuid().ToString("N"), TemplateFieldTypes.TEXT_FIELD);
		}

		[TestFixtureTearDown]
		public void TearDownFixture()
		{
			TemplateCreateUtility.CleanUpTestTemplatesFolder();
		}

		#region Template Inclusion
		[Test]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplatesByPath()
		{
			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(TemplateCreateUtility.TestTemplatePath);

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(4, templates.Count());
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplatesByFolderID()
		{
			var tempTemplatesFolderId = TemplateCreateUtility.TestTemplateFolder.ID.ToString();

			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(tempTemplatesFolderId);

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(4, templates.Count());
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplateByTemplateID()
		{
			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(AliceTemplate.ID.ToString());

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(1, templates.Count());
			Assert.AreEqual(templates.First().Name, AliceTemplate.Name);
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplateByName()
		{
			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(FlowerChildTemplate.Name);

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(1, templates.Count());
			Assert.AreEqual(templates.First().Name, FlowerChildTemplate.Name);
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplatesByWildcardPath()
		{
			var wildcard = TemplateCreateUtility.TestTemplatePath + "/A*"; // should include Alice and Aliceson

			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(wildcard);

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(2, templates.Count());
			Assert.IsTrue(templates.Any(x => x.Name == AliceTemplate.Name));
			Assert.IsTrue(templates.Any(x => x.Name == AlicesonTemplate.Name));
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplatesBySingleWildcardPath()
		{
			var wildcard = TemplateCreateUtility.TestTemplatePath + "/A????" + AliceTemplate.Name.Substring(5); // should include Alice

			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(wildcard);

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(1, templates.Count());
			Assert.IsTrue(templates.First().Name == AliceTemplate.Name);
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplatesByWildcardName()
		{
			const string wildcard = "*Alice*"; // NOTE: this will fail if you have templates called "Alice." But why would you have that?

			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(wildcard);

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(2, templates.Count());
			Assert.IsTrue(templates.Any(x => x.Name == AliceTemplate.Name));
			Assert.IsTrue(templates.Any(x => x.Name == AlicesonTemplate.Name));
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplatesBySingleWildcardName()
		{
			var wildcard = "?????" + AliceTemplate.Name.Substring(5);

			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(wildcard);

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(1, templates.Count());
			Assert.IsTrue(templates.First().Name == AliceTemplate.Name);
		}
		#endregion

		// gets a provider that will return all four test templates unless exclusions are applied
		private ConfigurationTemplateInputProvider GetExclusionTestProvider()
		{
			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(TemplateCreateUtility.TestTemplatePath);

			return provider;
		}

		#region Template Exclusion
		[Test]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplatesByPath()
		{
			var provider = GetExclusionTestProvider();
			provider.AddTemplateExclusion(TemplateCreateUtility.TestTemplatePath);

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(0, templates.Count()); // excludes all templates
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplatesByFolderID()
		{
			var provider = GetExclusionTestProvider();
			provider.AddTemplateExclusion(TemplateCreateUtility.TestTemplateFolder.ID.ToString());

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(0, templates.Count()); // excludes all templates
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplateByTemplateID()
		{
			var provider = GetExclusionTestProvider();
			provider.AddTemplateExclusion(AliceTemplate.ID.ToString());

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(3, templates.Count());
			Assert.IsFalse(templates.Any(x => x.Name == AliceTemplate.Name));
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplateByName()
		{
			var provider = GetExclusionTestProvider();
			provider.AddTemplateExclusion(BobTemplate.Name);

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(3, templates.Count());
			Assert.IsFalse(templates.Any(x => x.Name == BobTemplate.Name));
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplatesByWildcardPath()
		{
			var wildcard = TemplateCreateUtility.TestTemplatePath + "/A*"; // should exclude Alice and Aliceson

			var provider = GetExclusionTestProvider();
			provider.AddTemplateExclusion(wildcard);

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(2, templates.Count());
			Assert.IsFalse(templates.Any(x => x.Name == AliceTemplate.Name));
			Assert.IsFalse(templates.Any(x => x.Name == AlicesonTemplate.Name));
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplatesBySingleWildcardPath()
		{
			var wildcard = TemplateCreateUtility.TestTemplatePath + "/A????" + AliceTemplate.Name.Substring(5); // should include Alice

			var provider = GetExclusionTestProvider();
			provider.AddTemplateExclusion(wildcard);

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(3, templates.Count());
			Assert.IsFalse(templates.Any(x => x.Name == AliceTemplate.Name));
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplatesByWildcardName()
		{
			const string wildcard = "*Alice*"; // NOTE: this will fail if you have templates called "Alice." But why would you have that?

			var provider = GetExclusionTestProvider();
			provider.AddTemplateExclusion(wildcard);

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(2, templates.Count());
			Assert.IsFalse(templates.Any(x => x.Name == AliceTemplate.Name));
			Assert.IsFalse(templates.Any(x => x.Name == AlicesonTemplate.Name));
		}

		[Test]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplatesBySingleWildcardName()
		{
			var wildcard = "?????" + AliceTemplate.Name.Substring(5);

			var provider = GetExclusionTestProvider();
			provider.AddTemplateExclusion(wildcard);

			var templates = provider.CreateTemplateList();

			Assert.AreEqual(3, templates.Count());
			Assert.IsFalse(templates.Any(x => x.Name == AliceTemplate.Name));
		}
		#endregion

		#region Field Exclusion
		#region Global Name Exclusion
		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_GlobalName_Included()
		{
			var provider = GetExclusionTestProvider();

			Assert.IsTrue(provider.IsFieldIncluded(FlowerChildField.ID));
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_GlobalName_ExcludedByPath()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceField.Paths.FullPath);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_GlobalName_ExcludedByName()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AlicesonField.Name);

			Assert.IsFalse(provider.IsFieldIncluded(AlicesonField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_GlobalName_ExcludedById()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceField.ID.ToString());

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_GlobalName_ExcludedByNameWildcard()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + AliceField.Name.Substring(5));

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_GlobalName_ExcludedByPathWildcard()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceField.Parent.Paths.FullPath + "*");

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}
		#endregion

		#region Template Spec by ID
		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByPath()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.ID + "::" + AliceField.Paths.FullPath);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByPath_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.ID + "::" + AliceField.Paths.FullPath);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByName()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AlicesonTemplate.ID + "::" + AlicesonField.Name);

			Assert.IsFalse(provider.IsFieldIncluded(AlicesonField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByName_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.ID + "::" + AlicesonField.Name);

			Assert.IsFalse(provider.IsFieldIncluded(AlicesonField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedById()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.ID + "::" + AliceField.ID);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedById_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.ID + "::" + AliceField.ID);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByNameWildcard()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.ID + "::" + "*" + AliceField.Name.Substring(5));

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByNameWildcard_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.ID + "::" + "*" + AliceField.Name.Substring(5));

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByPathWildcard()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.ID + "::" + AliceField.Parent.Paths.FullPath + "*");

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByPathWildcard_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.ID + "::" + AliceField.Parent.Paths.FullPath + "*");

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}
		#endregion

		#region Template Spec by Name
		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByPath()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.Name + "::" + AliceField.Paths.FullPath);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByPath_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Name + "::" + AliceField.Paths.FullPath);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByName()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AlicesonTemplate.Name + "::" + AlicesonField.Name);

			Assert.IsFalse(provider.IsFieldIncluded(AlicesonField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByName_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Name + "::" + AlicesonField.Name);

			Assert.IsFalse(provider.IsFieldIncluded(AlicesonField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedById()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.Name + "::" + AliceField.ID);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedById_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Name + "::" + AliceField.ID);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByNameWildcard()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.Name + "::" + "*" + AliceField.Name.Substring(5));

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByNameWildcard_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Name + "::" + "*" + AliceField.Name.Substring(5));

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByPathWildcard()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.Name + "::" + AliceField.Parent.Paths.FullPath + "*");

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByPathWildcard_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Name + "::" + AliceField.Parent.Paths.FullPath + "*");

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}
		#endregion

		#region Template Spec by Path
		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByPath()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.Paths.FullPath + "::" + AliceField.Paths.FullPath);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByPath_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Paths.FullPath + "::" + AliceField.Paths.FullPath);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByName()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AlicesonTemplate.Paths.FullPath + "::" + AlicesonField.Name);

			Assert.IsFalse(provider.IsFieldIncluded(AlicesonField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByName_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Paths.FullPath + "::" + AlicesonField.Name);

			Assert.IsFalse(provider.IsFieldIncluded(AlicesonField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedById()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.Paths.FullPath + "::" + AliceField.ID);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedById_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Paths.FullPath + "::" + AliceField.ID);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByNameWildcard()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.Paths.FullPath + "::" + "*" + AliceField.Name.Substring(5));

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByNameWildcard_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Paths.FullPath + "::" + "*" + AliceField.Name.Substring(5));

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByPathWildcard()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.Paths.FullPath + "::" + AliceField.Parent.Paths.FullPath + "*");

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByPathWildcard_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Paths.FullPath + "::" + AliceField.Parent.Paths.FullPath + "*");

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}
		#endregion

		#region Template Spec by Name Wildcard
		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByPath()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + AliceTemplate.Name.Substring(5) + "::" + AliceField.Paths.FullPath);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByPath_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + FlowerChildTemplate.Name.Substring(5) + "::" + AliceField.Paths.FullPath);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByName()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + AlicesonTemplate.Name.Substring(5) + "::" + AlicesonField.Name);

			Assert.IsFalse(provider.IsFieldIncluded(AlicesonField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByName_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + FlowerChildTemplate.Name.Substring(5) + "::" + AlicesonField.Name);

			Assert.IsFalse(provider.IsFieldIncluded(AlicesonField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedById()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + AliceTemplate.Name.Substring(5) + "::" + AliceField.ID);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedById_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + FlowerChildTemplate.Name.Substring(5) + "::" + AliceField.ID);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByNameWildcard()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + AliceTemplate.Name.Substring(5) + "::" + "*" + AliceField.Name.Substring(5));

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByNameWildcard_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + FlowerChildTemplate.Name.Substring(5) + "::" + "*" + AliceField.Name.Substring(5));

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByPathWildcard()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + AliceTemplate.Name.Substring(5) + "::" + AliceField.Parent.Paths.FullPath + "*");

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByPathWildcard_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + FlowerChildTemplate.Name.Substring(5) + "::" + AliceField.Parent.Paths.FullPath + "*");

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}
		#endregion

		#region Template Spec by Path Wildcard
		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByPath()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + AliceTemplate.Name.Substring(5) + "::" + AliceField.Paths.FullPath);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByPath_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + FlowerChildTemplate.Name.Substring(5) + "::" + AliceField.Paths.FullPath);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByName()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AlicesonTemplate.Parent.Paths.FullPath + "*" + "::" + AlicesonField.Name);

			Assert.IsFalse(provider.IsFieldIncluded(AlicesonField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByName_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Parent.Paths.FullPath + "*" + "::" + AlicesonField.Name);

			Assert.IsFalse(provider.IsFieldIncluded(AlicesonField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedById()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.Parent.Paths.FullPath + "*" + "::" + AliceField.ID);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedById_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Parent.Paths.FullPath + "*" + "::" + AliceField.ID);

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByNameWildcard()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.Parent.Paths.FullPath + "*" + "::" + "*" + AliceField.Name.Substring(5));

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByNameWildcard_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Parent.Paths.FullPath + "*" + "::" + "*" + AliceField.Name.Substring(5));

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByPathWildcard()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceTemplate.Parent.Paths.FullPath + "*" + "::" + AliceField.Parent.Paths.FullPath + "*");

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}

		[Test]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByPathWildcard_WhenInherited()
		{
			var provider = GetExclusionTestProvider();
			provider.AddFieldExclusion(FlowerChildTemplate.Parent.Paths.FullPath + "*" + "::" + AliceField.Parent.Paths.FullPath + "*");

			Assert.IsFalse(provider.IsFieldIncluded(AliceField.ID));
			Assert.IsTrue(provider.IsFieldIncluded(BobField.ID), "Sanity check - bob field should never be excluded");
		}
		#endregion
		#endregion
	}
}
