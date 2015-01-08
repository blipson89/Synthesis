using Sitecore.Data;

namespace Synthesis.Templates
{
	public interface ITemplateFieldInfo
	{
		string Name { get; }
		string DisplayName { get; }
		string FullPath { get; }
		ID Id { get; }
		string HelpText { get; }
		string Type { get; }
		ITemplateInfo Template { get; }
	}
}
