using System;
using System.Linq;
using FluentAssertions;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Synthesis.Configuration;
using Synthesis.Tests.Fixtures.Configuration;
using Synthesis.Tests.Utility;
using Xunit;

namespace Synthesis.Tests.Configuration
{
    [Trait("Category", "Configuration Tests")]
    public class TemplateInputProviderTests : IDisposable
	{
		private readonly DbTemplate _aliceTemplate; // the alice template is a parent template with fields "Hello"
        private readonly DbTemplate _bobTemplate; // the bob template is a parent template with fields "Hi"
        private readonly DbTemplate _allisonTemplate; // the allison template is  child template that inherits from alice and bob with fields "Child"
        private readonly DbTemplate _flowerChildTemplate; // the flowerchild template inherits from the allison template with fields "Grandchild"

        private Item AliceField => db.GetItem(_aliceTemplate.Fields.First().ID);
        private Item BobField => db.GetItem(_bobTemplate.Fields.First().ID);
        private Item AllisonField => db.GetItem(_allisonTemplate.Fields.First().ID);
        private Item FlowerChildField => db.GetItem(_flowerChildTemplate.Fields.First().ID);
        private Db db;
        private readonly TemplateInputTestFixture _fixture;
        
        public TemplateInputProviderTests()
        {
            _fixture = new TemplateInputTestFixture();
            _aliceTemplate = _fixture.AliceTemplate;
            _bobTemplate = _fixture.BobTemplate; // the bob template is a parent template with fields "Hi"
            _allisonTemplate = _fixture.AllisonTemplate; // the allison template is  child template that inherits from alice and bob with fields "Child"
            _flowerChildTemplate = _fixture.FlowerChildTemplate;
            db = _fixture._db;
        }

		public void Dispose()
		{
            _fixture.Dispose();
        }

		#region Template Inclusion
		[Fact]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplatesByPath()
		{
			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(TestingContants.TestTemplatePath);

			var templates = provider.CreateTemplateList();

            templates.Count().Should().Be(4);
        }

		[Fact]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplatesByFolderID()
		{
			var tempTemplatesFolderId = TestingContants.TestTemplateFolder.ID.ToString();

			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(tempTemplatesFolderId);

			var templates = provider.CreateTemplateList();

            templates.Count().Should().Be(4);
        }

		[Fact]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplateByTemplateID()
		{
			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(_aliceTemplate.ID.ToString());

			var templates = provider.CreateTemplateList().ToArray();

            templates.Length.Should().Be(1);
            templates.First().Name.Should().Be(_aliceTemplate.Name);
		}

		[Fact]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplateByName()
		{
			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(_flowerChildTemplate.Name);

			var templates = provider.CreateTemplateList().ToArray();

            templates.Length.Should().Be(1);
            templates.First().Name.Should().Be(_flowerChildTemplate.Name);
		}

		[Fact]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplatesByWildcardPath()
		{
			var wildcard = TestingContants.TestTemplatePath + "/A*"; // should include Alice and Allison

			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(wildcard);

			var templates = provider.CreateTemplateList().ToArray();

            templates.Length.Should().Be(2);
            templates.Should().Contain(t => t.Name == _aliceTemplate.Name);
            templates.Should().Contain(x => x.Name == _allisonTemplate.Name);
		}

		[Fact]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplatesBySingleWildcardPath()
		{
			var wildcard = TestingContants.TestTemplatePath + "/A????" + _aliceTemplate.Name.Substring(5); // should include Alice

			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(wildcard);

            var templates = provider.CreateTemplateList().ToArray();

            templates.Length.Should().Be(1);
            templates.First().Name.Should().Be(_aliceTemplate.Name);
		}

		[Fact]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplatesByWildcardName()
		{
			const string wildcard = "*Al*"; 

			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(wildcard);

			var templates = provider.CreateTemplateList().ToArray();

            templates.Length.Should().Be(2);
            templates.Should().Contain(x => x.Name == _aliceTemplate.Name);
            templates.Should().Contain(x => x.Name == _allisonTemplate.Name);
		}

		[Fact]
		public void TemplateInputProvider_GetTemplates_IncludesExpectedTemplatesBySingleWildcardName()
		{
			var wildcard = "?????" + _aliceTemplate.Name.Substring(5);

			var provider = new ConfigurationTemplateInputProvider();
			provider.AddTemplatePath(wildcard);

            var templates = provider.CreateTemplateList().ToArray();

            templates.Length.Should().Be(1);
            templates.First().Name.Should().Be(_aliceTemplate.Name);
		}
		#endregion

		#region Template Exclusion
		[Fact]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplatesByPath()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddTemplateExclusion(TestingContants.TestTemplatePath);

			var templates = provider.CreateTemplateList();

            templates.Count().Should().Be(0); // excludes all templates
		}

		[Fact]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplatesByFolderID()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddTemplateExclusion(TestingContants.TestTemplateFolder.ID.ToString());

			var templates = provider.CreateTemplateList();

            templates.Count().Should().Be(0); // excludes all templates
		}

		[Fact]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplateByTemplateID()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddTemplateExclusion(_aliceTemplate.ID.ToString());

			var templates = provider.CreateTemplateList().ToArray();

            templates.Length.Should().Be(3);
            templates.Should().NotContain(x => x.Name == _aliceTemplate.Name);
		}

		[Fact]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplateByName()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddTemplateExclusion(_bobTemplate.Name);

			var templates = provider.CreateTemplateList().ToArray();

            templates.Length.Should().Be(3);
            templates.Should().NotContain(x => x.Name == _bobTemplate.Name);
		}

		[Fact]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplatesByWildcardPath()
		{
			var wildcard = TestingContants.TestTemplatePath + "/A*"; // should exclude Alice and Allison

			var provider = _fixture.GetExclusionTestProvider();
			provider.AddTemplateExclusion(wildcard);

            var templates = provider.CreateTemplateList().ToArray();

            templates.Length.Should().Be(2);
            templates.Should().NotContain(x => x.Name == _aliceTemplate.Name);
            templates.Should().NotContain(x => x.Name == _allisonTemplate.Name);
		}

		[Fact]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplatesBySingleWildcardPath()
		{
			var wildcard = TestingContants.TestTemplatePath + "/A????" + _aliceTemplate.Name.Substring(5); // should include Alice

			var provider = _fixture.GetExclusionTestProvider();
			provider.AddTemplateExclusion(wildcard);

			var templates = provider.CreateTemplateList().ToArray();

            templates.Length.Should().Be(3);
            templates.Should().NotContain(x => x.Name == _aliceTemplate.Name);
        }

		[Fact]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplatesByWildcardName()
		{
			const string wildcard = "*Al*";

			var provider = _fixture.GetExclusionTestProvider();
			provider.AddTemplateExclusion(wildcard);

			var templates = provider.CreateTemplateList().ToArray();

            templates.Length.Should().Be(2);
            templates.Should().NotContain(x => x.Name == _aliceTemplate.Name);
            templates.Should().NotContain(x => x.Name == _allisonTemplate.Name);
		}

		[Fact]
		public void TemplateInputProvider_GetTemplates_ExcludesExpectedTemplatesBySingleWildcardName()
		{
			var wildcard = "?????" + _aliceTemplate.Name.Substring(5);

			var provider = _fixture.GetExclusionTestProvider();
			provider.AddTemplateExclusion(wildcard);

			var templates = provider.CreateTemplateList().ToArray();

            templates.Length.Should().Be(3);
            templates.Should().NotContain(x => x.Name == _aliceTemplate.Name);
		}
		#endregion

		#region Field Exclusion
		#region Global Name Exclusion
		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_GlobalName_Included()
		{
			var provider = _fixture.GetExclusionTestProvider();

            provider.IsFieldIncluded(FlowerChildField.ID).Should().BeTrue();
        }

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_GlobalName_ExcludedByPath()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceField.Paths.FullPath);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
            provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
        }

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_GlobalName_ExcludedByName()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(AllisonField.Name);

            provider.IsFieldIncluded(AllisonField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_GlobalName_ExcludedById()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceField.ID.ToString());

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
            provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
        }

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_GlobalName_ExcludedByNameWildcard()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + AliceField.Name.Substring(5));

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
            provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
        }

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_GlobalName_ExcludedByPathWildcard()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(AliceField.Parent.Paths.FullPath + "*");

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}
		#endregion

		#region Template Spec by ID
		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByPath()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_aliceTemplate.ID + "::" + AliceField.Paths.FullPath);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByPath_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.ID + "::" + AliceField.Paths.FullPath);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByName()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_allisonTemplate.ID + "::" + AllisonField.Name);

            provider.IsFieldIncluded(AllisonField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByName_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.ID + "::" + AllisonField.Name);

            provider.IsFieldIncluded(AllisonField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedById()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_aliceTemplate.ID + "::" + AliceField.ID);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedById_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.ID + "::" + AliceField.ID);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByNameWildcard()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_aliceTemplate.ID + "::" + "*" + AliceField.Name.Substring(5));

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByNameWildcard_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.ID + "::" + "*" + AliceField.Name.Substring(5));

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByPathWildcard()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_aliceTemplate.ID + "::" + AliceField.Parent.Paths.FullPath + "*");

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByID_ExcludedByPathWildcard_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.ID + "::" + AliceField.Parent.Paths.FullPath + "*");

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}
		#endregion

		#region Template Spec by Name
		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByPath()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_aliceTemplate.Name + "::" + AliceField.Paths.FullPath);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByPath_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.Name + "::" + AliceField.Paths.FullPath);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByName()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_allisonTemplate.Name + "::" + AllisonField.Name);

            provider.IsFieldIncluded(AllisonField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByName_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.Name + "::" + AllisonField.Name);

            provider.IsFieldIncluded(AllisonField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedById()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_aliceTemplate.Name + "::" + AliceField.ID);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedById_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.Name + "::" + AliceField.ID);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByNameWildcard()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_aliceTemplate.Name + "::" + "*" + AliceField.Name.Substring(5));

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByNameWildcard_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.Name + "::" + "*" + AliceField.Name.Substring(5));

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByPathWildcard()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_aliceTemplate.Name + "::" + AliceField.Parent.Paths.FullPath + "*");

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByName_ExcludedByPathWildcard_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.Name + "::" + AliceField.Parent.Paths.FullPath + "*");

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}
		#endregion

		#region Template Spec by Path
		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByPath()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_aliceTemplate.FullPath + "::" + AliceField.Paths.FullPath);


            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByPath_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.FullPath + "::" + AliceField.Paths.FullPath);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByName()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_allisonTemplate.FullPath + "::" + AllisonField.Name);

            provider.IsFieldIncluded(AllisonField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByName_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.FullPath + "::" + AllisonField.Name);

            provider.IsFieldIncluded(AllisonField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedById()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_aliceTemplate.FullPath + "::" + AliceField.ID);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedById_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.FullPath + "::" + AliceField.ID);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByNameWildcard()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_aliceTemplate.FullPath + "::" + "*" + AliceField.Name.Substring(5));

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue( "Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByNameWildcard_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.FullPath + "::" + "*" + AliceField.Name.Substring(5));

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByPathWildcard()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_aliceTemplate.FullPath + "::" + AliceField.Parent.Paths.FullPath + "*");

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPath_ExcludedByPathWildcard_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_flowerChildTemplate.FullPath + "::" + AliceField.Parent.Paths.FullPath + "*");

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}
		#endregion

		#region Template Spec by Name Wildcard
		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByPath()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + _aliceTemplate.Name.Substring(5) + "::" + AliceField.Paths.FullPath);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByPath_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + _flowerChildTemplate.Name.Substring(5) + "::" + AliceField.Paths.FullPath);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByName()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + _allisonTemplate.Name.Substring(5) + "::" + AllisonField.Name);

            provider.IsFieldIncluded(AllisonField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue( "Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByName_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + _flowerChildTemplate.Name.Substring(5) + "::" + AllisonField.Name);

            provider.IsFieldIncluded(AllisonField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedById()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + _aliceTemplate.Name.Substring(5) + "::" + AliceField.ID);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedById_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + _flowerChildTemplate.Name.Substring(5) + "::" + AliceField.ID);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByNameWildcard()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + _aliceTemplate.Name.Substring(5) + "::" + "*" + AliceField.Name.Substring(5));

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
        }

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByNameWildcard_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + _flowerChildTemplate.Name.Substring(5) + "::" + "*" + AliceField.Name.Substring(5));

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByPathWildcard()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + _aliceTemplate.Name.Substring(5) + "::" + AliceField.Parent.Paths.FullPath + "*");

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByNameWildcard_ExcludedByPathWildcard_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + _flowerChildTemplate.Name.Substring(5) + "::" + AliceField.Parent.Paths.FullPath + "*");

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}
		#endregion

		#region Template Spec by Path Wildcard
		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByPath()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + _aliceTemplate.Name.Substring(5) + "::" + AliceField.Paths.FullPath);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByPath_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion("*" + _flowerChildTemplate.Name.Substring(5) + "::" + AliceField.Paths.FullPath);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

        

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByName()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_fixture.ParentFullPath(_allisonTemplate) + "*" + "::" + AllisonField.Name);

            provider.IsFieldIncluded(AllisonField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByName_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_fixture.ParentFullPath(_flowerChildTemplate) + "*" + "::" + AllisonField.Name);

            provider.IsFieldIncluded(AllisonField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedById()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_fixture.ParentFullPath(_aliceTemplate) + "*" + "::" + AliceField.ID);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedById_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_fixture.ParentFullPath(_flowerChildTemplate) + "*" + "::" + AliceField.ID);

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByNameWildcard()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_fixture.ParentFullPath(_aliceTemplate) + "*" + "::" + "*" + AliceField.Name.Substring(5));

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
        }

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByNameWildcard_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_fixture.ParentFullPath(_flowerChildTemplate) + "*" + "::" + "*" + AliceField.Name.Substring(5));

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByPathWildcard()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_fixture.ParentFullPath(_aliceTemplate) + "*" + "::" + AliceField.Parent.Paths.FullPath + "*");

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}

		[Fact]
		public void TemplateInputProvider_ShouldGeneratePropertyForField_TemplateByPathWildcard_ExcludedByPathWildcard_WhenInherited()
		{
			var provider = _fixture.GetExclusionTestProvider();
			provider.AddFieldExclusion(_fixture.ParentFullPath(_flowerChildTemplate) + "*" + "::" + AliceField.Parent.Paths.FullPath + "*");

            provider.IsFieldIncluded(AliceField.ID).Should().BeFalse();
			provider.IsFieldIncluded(BobField.ID).Should().BeTrue("Sanity check - bob field should never be excluded");
		}
		#endregion
		#endregion
	}
}
