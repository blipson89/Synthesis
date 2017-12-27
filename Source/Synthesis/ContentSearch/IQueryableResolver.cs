using System.Linq;
using Synthesis.Pipelines;

namespace Synthesis.ContentSearch
{
	public interface IQueryableResolver
	{
		IQueryable<TResult> GetSynthesisQueryable<TResult>(SynthesisSearchContextArgs args)
			where TResult : IStandardTemplateItem;
	}
}
