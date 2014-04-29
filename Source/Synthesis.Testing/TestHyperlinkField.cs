using System;
using Sitecore.Data.Fields;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Testing
{
	public class TestHyperlinkField : TestFieldType, IHyperlinkField
	{
		public TestHyperlinkField(string href, string text = null, string cssClass = null, string target = null, IStandardTemplateItem targetItem = null)
		{
			Href = href;
			Text = text;
			CssClass = cssClass;
			Target = target;
			TargetItem = targetItem;
		}

		/// <summary>
		///     Checks if the field has a non-empty Href value
		/// </summary>
		public override bool HasValue
		{
			get { return !string.IsNullOrEmpty(Href); }
		}

		/// <summary>
		///     The CSS class of the link, if one was entered
		/// </summary>
		public string CssClass { get; set; }

		/// <summary>
		///     Gets the link URL. Handles internal links, external, media, etc types.
		/// </summary>
		public string Href { get; set; }

		/// <summary>
		///     The target attribute of the link, if one was entered. Note that "_blank" is interpreted as rel="external" by default for XHTML compatibility.
		/// </summary>
		public string Target { get; set; }

		/// <summary>
		///     The text of the link, if one was entered.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		///     The title attribute of the link, if one was entered
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		///     Renders the field using a Sitecore FieldRenderer and returns the result
		/// </summary>
		public virtual string RenderedValue
		{
			get { return string.Format("<a href=\"{0}\" class=\"{1}\" target=\"{2}\" title=\"{3}\">{4}</a>", Href ?? string.Empty, CssClass ?? string.Empty, Target ?? string.Empty, Title ?? string.Empty, Text ?? string.Empty); }
		}

		/// <summary>
		///     Gets the target Sitecore item of this link (if the link is internal)
		/// </summary>
		public IStandardTemplateItem TargetItem { get; private set; }

		public LinkField ToLinkField()
		{
			throw new NotImplementedException("Test date fields cannot return Sitecore item objects");
		}

		/// <summary>
		///     Shows the URL of the hyperlink
		/// </summary>
		public override string ToString()
		{
			return Href;
		}
	}
}