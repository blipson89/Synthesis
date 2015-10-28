using Sitecore.Data;

namespace Synthesis.Initializers
{
	public interface IInitializerProvider
	{
		ITemplateInitializer GetInitializer(ID templateId);
	}
}
