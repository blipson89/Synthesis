using System.Linq;
using Blade;
using Sitecore.Data.Items;
namespace Synthesis.Blade
{
	/// <summary>
	/// A generic list-based presenter that is tied to a Synthesis model type. This is usually a good starting point to derive from if you need to modify presentation behavior.
	/// </summary>
	/// <typeparam name="TModel">Type of the ViewModel that will be returned to the view</typeparam>
	/// <typeparam name="TItem">Type of Synthesis item the presenter expects as a data source</typeparam>
	public abstract class SynthesisListPresenter<TModel, TItem> : SitecoreListPresenter<TModel>
		where TModel : class
		where TItem : class, IStandardTemplateItem
	{
		protected override TModel GetModel(IView view, Item[] dataSource)
		{
			return GetModel(view, dataSource.AsStronglyTypedCollectionOf<TItem>().ToArray());
		}

		protected abstract TModel GetModel(IView view, TItem[] items);
	}
}
