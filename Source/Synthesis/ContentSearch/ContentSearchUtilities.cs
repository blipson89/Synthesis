using System.Linq;
using Sitecore.Data;

namespace Synthesis.ContentSearch
{
	internal static class ContentSearchUtilities
	{
		internal static ID[] ParseIndexIdList(string indexValue)
		{
			return indexValue.Split('|')
					.Select(x =>
					{
						ShortID id;
						if (!ShortID.TryParse(x, out id)) return null;
						return id.ToID();
					})
					.Where(x => x != (ID)null)
					.ToArray();
		}
	}
}
