using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.Pipelines;

namespace Synthesis.Pipelines
{
	public class SynthesisSearchContextArgs : PipelineArgs
	{
		public IExecutionContext[] ExecutionContext { get; set; }
		public IProviderSearchContext SearchContext { get; set; }
	}
}
