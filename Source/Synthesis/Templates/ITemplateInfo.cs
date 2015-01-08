using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sitecore.Data;

namespace Synthesis.Templates
{
	public interface ITemplateInfo
	{
		ID TemplateId { get; }
		string Name { get; }
		IList<ITemplateFieldInfo> Fields { get; }
		string HelpText { get; }
		string FullPath { get; }
		IList<ITemplateFieldInfo> OwnFields { get; }

		/// <summary>
		/// Gets immediate ancestor base templates
		/// </summary>
		ReadOnlyCollection<ITemplateInfo> BaseTemplates { get; }

		/// <summary>
		/// Gets all base templates (including base templates of base templates back up to the standard template)
		/// </summary>
		ReadOnlyCollection<ITemplateInfo> AllNonstandardBaseTemplates { get; }
	}
}