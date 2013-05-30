namespace Synthesis.Synchronization
{
	/// <summary>
	/// Indicates where a sync-checked item exists
	/// </summary>
	public enum SyncSource 
	{  
		/// <summary>
		/// The template exists only in Sitecore
		/// </summary>
		Sitecore, 
		/// <summary>
		/// The template exists only in the Model
		/// </summary>
		Model, 
		/// <summary>
		/// The template exists in both places. Does NOT imply that it is necessarily synchronized.
		/// </summary>
		Both 
	}
}