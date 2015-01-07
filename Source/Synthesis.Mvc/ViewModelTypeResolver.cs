using System;
using System.Web.Compilation;

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
			var compiledViewType = BuildManager.GetCompiledType(viewPath);
			var baseType = compiledViewType.BaseType;

			// Check to see if the view has been found and that it is a generic type
			if (baseType == null || !baseType.IsGenericType) return typeof(object);

			return baseType.GetGenericArguments()[0];
		}
	}
}
