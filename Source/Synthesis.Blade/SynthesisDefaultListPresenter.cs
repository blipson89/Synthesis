using System.Collections.Generic;
using System.Linq;
using Blade;
using Sitecore.Data.Items;

namespace Synthesis.Blade
{
	/// <summary>
	/// Generic presenter that is used when an IEnumerable[SynthesisType] is requested as a model. Resolves the model using the rendering data source or sitecore context.
	/// </summary>
	/// <typeparam name="TModel">The type of the model list</typeparam>
	/// <typeparam name="TElement">The Synthesis type expected to be the element in the returned model list</typeparam>
	internal class SynthesisDefaultListPresenter<TModel, TElement> : SitecoreListPresenter<TModel>
		where TModel : class, IEnumerable<TElement>
		where TElement: class, IStandardTemplateItem
	{
        protected override TModel GetModel(IView view, Item[] dataSource)
        {
	        return (TModel)(IEnumerable<TElement>)dataSource.AsStronglyTypedCollectionOf<TElement>().ToArray();
        }
    }
}
