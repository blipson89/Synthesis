using System;
using System.Linq;
using Sitecore.Data.Items;

namespace Synthesis.FieldTypes.Adapters
{
	public class StatisticsAdapter : IStatisticsAdapter
	{
		private readonly ItemStatistics _statistics;

		public StatisticsAdapter(ItemStatistics statistics)
		{
			_statistics = statistics;
		}

		public DateTime Created { get { return _statistics.Created; } }
		public string CreatedBy { get { return _statistics.CreatedBy; } }
		public DateTime Updated { get { return _statistics.Updated; } }
		public string UpdatedBy { get { return _statistics.UpdatedBy; } }

		public string Revision { get { return _statistics.Revision; } }
	}
}
