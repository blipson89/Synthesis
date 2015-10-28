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

		public string Name
		{
			get { return _field.Name; }
		}

		public string DisplayName
		{
			get { return _field.DisplayName; }
		}

		public string FullPath
		{
			get { return _field.InnerItem.Paths.FullPath; }
		}

		public ID Id
		{
			get { return _field.ID; }
		}

		public string HelpText
		{
			get { return _field.Description; }
		}

		public string Type
		{
			get { return _field.Type; }
		}

		public ITemplateInfo Template { get; private set; }
	}
}
