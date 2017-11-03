using Sitecore.ContentSearch;

namespace Synthesis.Mvc.UI
{
	public interface IContextIndex
	{
		/// <summary>
		/// Gets the Sitecore index for the context item
		/// </summary>
		ISearchIndex ContextIndex { get; }
	}
}
