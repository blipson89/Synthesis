using System;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Synthesis.Tests.Utility
{
	internal class TestItemContext : IDisposable
	{
		const string ROOT_PATH = "/sitecore/content";
		
		internal TestItemContext()
		{
			var testName = "test-" + ID.NewID.ToShortID();
			var database = Factory.GetDatabase("master");

			using (new SecurityDisabler())
			{
				var parent = database.GetItem(ROOT_PATH);
				TestItem = parent.Add(testName, new TemplateID(FieldTestTemplateCreator.CurrentTestTemplateID));
			}
		}

		internal Item TestItem { get; private set; }

		internal void SetField(string fieldName, string fieldValue)
		{
			using (new SecurityDisabler())
			{
				using (new EditContext(TestItem))
				{
					TestItem[fieldName] = fieldValue;
				}
			}
		}

		internal Field this[string fieldname]
		{
			get { return TestItem.Fields[fieldname]; }
		}

		public void Dispose()
		{
			if (TestItem == null) return;

			using (new SecurityDisabler())
			{
				TestItem.Delete();
			}
		}
	}
}
