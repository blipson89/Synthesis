namespace Synthesis.Mvc.UI
{
	public interface IContextItem
	{
		TItem GetContextItem<TItem>()
			where TItem : class, IStandardTemplateItem;

		IStandardTemplateItem ContextItem { get; }
	}
}
