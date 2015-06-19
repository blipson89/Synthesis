using System;
using System.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq.Common;
using Sitecore.Pipelines;

namespace Synthesis.Pipelines
{
	public class SynthesisSearchContextArgs : PipelineArgs
	{
		public IQueryable SynthesisQueryable { get; set; }
		public IExecutionContext[] ExecutionContext { get; set; }
		public IProviderSearchContext SearchContext { get; set; }
		public Type SynthesisQueryType { get; set; }
	}
}
