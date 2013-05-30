using System;
using System.Collections.Generic;
using System.Linq;
using Blade.Configuration;
using System.Diagnostics.CodeAnalysis;


namespace Synthesis.Blade.Configuration
{
	public class SynthesisPresenterFactory : ConfigurationPresenterFactory
	{
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification="By design")]
		protected override Type GetDefaultPresenter<TModel>()
		{
			// resolve a model that is an IEnumerable<SynthesisType> and use the default list presenter
			// NOTE: only direct models of IEnumerable<T> are allowed - no implementers like T[] or List<T>
			var modelType = typeof (TModel);
			if (modelType.IsGenericType && modelType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
			{
				var genericElementType = modelType.GetGenericArguments().First();
				if (genericElementType.GetInterfaces().Contains(typeof (IStandardTemplateItem)) || genericElementType == typeof(IStandardTemplateItem))
				{
					return typeof (SynthesisDefaultListPresenter<,>).MakeGenericType(modelType, genericElementType);
				}
			}

			// return a normal single item default presenter
			return typeof(SynthesisDefaultPresenter<TModel>);
		}
	}
}
