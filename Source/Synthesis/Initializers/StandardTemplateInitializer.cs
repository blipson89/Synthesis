using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Synthesis.Initializers
{
	public class StandardTemplateInitializer : ITemplateInitializer
	{
		public IStandardTemplateItem CreateInstance(Item innerItem)
		{
			return new StandardTemplateItem(innerItem);
		}

		public IStandardTemplateItem CreateInstanceFromSearch(IDictionary<string, string> searchFields)
		{
			return new StandardTemplateItem(searchFields);
		}

		public ID InitializesTemplateId
		{
			get { return ID.Null; }
		}
	}
}
