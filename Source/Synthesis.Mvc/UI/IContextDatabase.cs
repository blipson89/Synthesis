using Synthesis.FieldTypes.Adapters;

namespace Synthesis.Mvc.UI
{
	public interface IContextDatabase
	{
		/// <summary>
		/// Gets the current context database.
		/// If the content database is set (e.g. when shell is the context site) returns that instead of the context database.
		/// </summary>
		IDatabaseAdapter ContextDatabase { get; }
	}
}
