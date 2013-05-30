using Blade;
using Sitecore.Data.Items;
namespace Synthesis.Blade
{
	/// <summary>
	/// A generic presenter that is tied to a Synthesis model type. This is usually a good starting point to derive from if you need to modify presentation behavior.
	/// </summary>
	/// <typeparam name="TModel">Type of the ViewModel that will be returned to the view</typeparam>
	/// <typeparam name="TItem">Type of Synthesis item the presenter expects as a data source</typeparam>
	public abstract class SynthesisPresenter<TModel, TItem> : SitecorePresenter<TModel>
		where TModel : class
		where TItem : class, IStandardTemplateItem
	{
		protected SynthesisPresenter() { }

        protected override TModel GetModel(IView view, Item dataSource)
        {
            return GetModel(view, dataSource.As<TItem>());
        }

        protected abstract TModel GetModel(IView view, TItem item);
    }
}
