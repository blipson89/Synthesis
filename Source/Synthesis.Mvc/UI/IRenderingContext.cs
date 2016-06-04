namespace Synthesis.Mvc.UI
{
	public interface IRenderingContext : IContextItem, IContextSite, IContextDatabase
	{
		TItem GetRenderingDatasource<TItem>()
			where TItem : class, IStandardTemplateItem;
	
		IStandardTemplateItem RenderingDatasource { get; }

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
