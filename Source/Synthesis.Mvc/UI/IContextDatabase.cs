using Synthesis.FieldTypes.Adapters;

namespace Synthesis.Mvc.UI
{
	public interface IContextDatabase
	{
		IDatabaseAdapter ContextDatabase { get; }
	}
}
