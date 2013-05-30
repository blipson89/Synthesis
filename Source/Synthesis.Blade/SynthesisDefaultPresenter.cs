using Blade;
using Sitecore.Data.Items;

namespace Synthesis.Blade
{
	/// <summary>
	/// Generic presenter that is used when a Synthesis type is requested as a model. Resolves the model using the rendering data source or sitecore context.
	/// </summary>
	/// <typeparam name="TModel">The model type/Synthesis type expected to return</typeparam>
	internal class SynthesisDefaultPresenter<TModel> : SitecorePresenter<TModel>
		where TModel : class
	{
        protected override TModel GetModel(IView view, Item dataSource)
        {
            return dataSource.AsStronglyTyped() as TModel;
        }
    }
}
