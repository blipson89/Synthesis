using Sitecore.Mvc.Presentation;

namespace Synthesis.Mvc.UI
{
	public interface IRenderingContext : IContextItem, IContextSite, IContextDatabase, IContextIndex
	{
		/// <summary>
		/// Gets the current rendering's datasource as an expected type
		/// </summary>
		/// <returns>The datasource, if it is set and of the correct template. Or null if not set or the wrong template. Returns the context item if the datasource is not set.</returns>
		TItem GetRenderingDatasource<TItem>()
			where TItem : class, IStandardTemplateItem;

		/// <summary>
		/// Gets the current rendering's datasource
		/// Returns the context item if the datasource is not set.
		/// </summary>
		IStandardTemplateItem RenderingDatasource { get; }

		/// <summary>
		/// Gets the current rendering's parameters.
		/// Returns empty parameters if no rendering is in context.
		/// </summary>
		RenderingParameters Parameters { get; }

		/// <summary>
		/// Is Experience Editor enabled (exclusive of IsPreview, IsNormal)
		/// </summary>
		bool IsEditing { get; }

		/// <summary>
		/// Is Preview enabled (exclusive of IsEditing, IsNormal)
		/// </summary>
		bool IsPreview { get; }

		/// <summary>
		/// Is Normal page rendering enabled (exclusive of IsEditing, IsPreview). NOTE: Debugging or Experience Explorer may be active when IsNormal=true.
		/// </summary>
		bool IsNormal { get; }

		/// <summary>
		/// Is the Experience Explorer active?
		/// </summary>
		bool IsExperienceExplorer { get; }

		/// <summary>
		/// Is the Sitecore Debugger active?
		/// </summary>
		bool IsDebugging { get; }

		/// <summary>
		/// Is the page rendering completely normal? In other words, both Normal page mode and NOT debugging and NOT experience explorer - what an end user would see.
		/// </summary>
		bool IsCompletelyNormal { get; }
	}
}
