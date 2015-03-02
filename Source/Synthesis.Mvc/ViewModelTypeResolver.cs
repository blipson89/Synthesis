using System;
using System.Web.Compilation;
using Sitecore.Diagnostics;

namespace Synthesis.Mvc
{
	/// <summary>
	/// Credit where due: this is based on Fortis' model resolver:
	/// https://github.com/Fortis-Collection/fortis/blob/master/Fortis.Mvc/Pipelines/GetModel/GetFromView.cs
	/// </summary>
	public class ViewModelTypeResolver
	{
		public virtual Type GetViewModelType(string viewPath)
		{
			if (string.IsNullOrWhiteSpace(viewPath)) return typeof(object);

			// Retrieve the compiled view
			try
			{
				var compiledViewType = BuildManager.GetCompiledType(viewPath);
				var baseType = compiledViewType.BaseType;

				// Check to see if the view has been found and that it is a generic type
				if (baseType == null || !baseType.IsGenericType) return typeof (object);

				return baseType.GetGenericArguments()[0];
			}
			catch (ArgumentException aex)
			{
				// this can occur if an invalid path (e.g. a relative path) is passed in; we ignore the error for now and let it pass
				// down to the MVC machinery to show a regular error message later
				Log.Error("Synthesis could not resolve the model type for view rendering " + viewPath, aex, this);
				return typeof (object);
			}
		}
	}
}
