using System.Reflection;
using FluentAssertions;
using Sitecore.ContentSearch;
using Sitecore.FakeDb;
using Synthesis.FieldTypes.Interfaces;
using Synthesis.Solr.ContentSearch;
using Synthesis.Testing.Attributes;
using Xunit;

namespace Synthesis.Solr.Tests.ContentSearch
{
    public class SynthesisSolrFieldNameTranslatorTests
    {
        [Theory, AutoNSubstitute]
        public void GetIndexFieldNameMemberInfo_IfMemberIsSynthesisFieldType_ReturnName(SynthesisSolrFieldNameTranslator sut)
        {
            using (new Db("master"))
            {
                PropertyInfo propertyInfo = typeof(MockItem).GetProperty(nameof(MockItem.TextField));
                string name = sut.GetIndexFieldName(propertyInfo);
                name.Should().BeEquivalentTo(propertyInfo.GetCustomAttribute<IndexFieldAttribute>().IndexFieldName);
            }
        }
    }
    public class MockItem
    {
        [IndexField("textfield_t")]
        public ITextField TextField { get; set; }
    }
}
