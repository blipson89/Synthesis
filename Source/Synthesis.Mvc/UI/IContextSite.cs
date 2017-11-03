using Sitecore.Sites;

namespace Synthesis.Mvc.UI
{
	public interface IContextSite
	{
		SiteContext ContextSite { get; }
	}
}
