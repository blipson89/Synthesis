namespace Synthesis.Mvc.UI
{
	public interface IContextItem
	{
		/// <summary>
		/// Gets the Sitecore context item (e.g. the current page) as a specified template
		/// </summary>
		/// <returns>The context item, or null if the context item is not set or is not the expected template.</returns>
		TItem GetContextItem<TItem>()
			where TItem : class, IStandardTemplateItem;
	}
}
