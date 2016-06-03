using System.Diagnostics;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Synthesis.Templates.Database
{
	[DebuggerDisplay("{Name}")]
	public class ItemTemplateFieldInfo : ITemplateFieldInfo
	{
		private readonly TemplateFieldItem _field;

		public ItemTemplateFieldInfo(TemplateFieldItem field)
		{
			Assert.ArgumentNotNull(field, "field");
			
			_field = field;
		}

		public ItemTemplateFieldInfo(TemplateFieldItem field, ITemplateInfo template) : this(field)
		{
			Assert.ArgumentNotNull(template, "template");
			
			Template = template;
		}

		public string Name => _field.Name;

		public string DisplayName => _field.DisplayName;

		public string FullPath => _field.InnerItem.Paths.FullPath;

		public ID Id => _field.ID;

		public string HelpText => _field.Description;

		public string Type => _field.Type;

		public ITemplateInfo Template { get; }
	}
}
