using Sitecore.ContentSearch;

namespace Synthesis.ContentSearch
{
    public interface IFieldNameTranslatorFactory
    {
        AbstractFieldNameTranslator GetFieldNameTranslator(ISearchIndex index);
    }
}
