using System;
using System.Web.Mvc;
using Sitecore.Diagnostics;
using Sitecore.Mvc;
using Synthesis.FieldTypes;

namespace Synthesis.Mvc
{
	public class TagRenderingContext<T> : IDisposable
	{
		private readonly HtmlHelper<T> _helper;

		public TagRenderingContext(HtmlHelper<T> helper, FieldType field, object parameters)
		{
			Assert.ArgumentNotNull(helper, nameof(helper));
			Assert.ArgumentNotNull(field, nameof(field));
			Assert.ArgumentNotNull(parameters, nameof(parameters));

			_helper = helper;

			var fieldValue = helper.Sitecore().BeginField(field.InnerField.ID.ToString(), field.InnerField.Item, parameters);

			helper.ViewContext.Writer.Write(fieldValue.ToHtmlString());
		}

		public void Dispose()
		{
			_helper.ViewContext.Writer.Write(_helper.Sitecore().EndField());
		}
	}
}
