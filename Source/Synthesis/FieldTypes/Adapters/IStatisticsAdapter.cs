using System;

namespace Synthesis.FieldTypes.Adapters
{
	public interface IStatisticsAdapter
	{
		DateTime Created { get; }
		string CreatedBy { get; }
		DateTime Updated { get; }
		string UpdatedBy { get; }
		string Revision { get; }
	}
}