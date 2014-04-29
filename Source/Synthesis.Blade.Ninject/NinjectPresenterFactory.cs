using System;
using Blade;
using Ninject;
using Ninject.Web.Common;
using Synthesis.Blade.Configuration;

namespace Synthesis.Blade.Ninject
{
	/// <summary>
	/// This presenter factory activates presenters by using Ninject to allow for constructor dependency injection
	/// </summary>
	public class NinjectPresenterFactory : SynthesisPresenterFactory
	{
		protected override IPresenter<TModel> ActivatePresenter<TModel>(Type presenterType)
		{
			IKernel kernel = new Bootstrapper().Kernel;
			return (IPresenter<TModel>)kernel.Get(presenterType);
		}
	}
}
