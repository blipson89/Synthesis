using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Synthesis.Synchronization
{
	/// <summary>
	/// A collection of synchronization results
	/// </summary>
	public class TemplateComparisonResultCollection : ReadOnlyCollection<TemplateComparisonResult>
	{
		private bool? _syncResult;

		public TemplateComparisonResultCollection(IList<TemplateComparisonResult> results) : base(results)
		{

		}

		/// <summary>
		/// Checks if all synchronization results in the collection are synchronized
		/// </summary>
		public bool AreTemplatesSynchronized
		{
			get
			{
				if (_syncResult == null)
					_syncResult = this.All(x => x.IsSynchronized);

				return _syncResult.Value;
			}
		}
	}
}
