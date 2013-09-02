using System.Linq;
using Sitecore.Data.Fields;

namespace Synthesis.FieldTypes.Interfaces
{
	public interface IHyperlinkField : IFieldType, IFieldRenderableFieldType
	{
		/// <summary>
		/// The CSS class of the link, if one was entered
		/// </summary>
		string CssClass { get; set; }

		/// <summary>
		/// Gets the link URL. Handles internal links, external, media, etc types.
		/// </summary>
		string Href { get; set; }

		/// <summary>
		/// The target attribute of the link, if one was entered. Note that "_blank" is interpreted as rel="external" by default for XHTML compatibility.
		/// </summary>
		string Target { get; set; }

		/// <summary>
		/// The text of the link, if one was entered. 
		/// </summary>
		string Text { get; set; }

		/// <summary>
		/// The title attribute of the link, if one was entered
		/// </summary>
		string Title { get; set; }

		/// <summary>
		/// Gets the target Sitecore item of this link (if the link is internal)
		/// </summary>
		IStandardTemplateItem TargetItem { get; }

		LinkField ToLinkField();
	}
}