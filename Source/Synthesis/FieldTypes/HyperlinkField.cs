using Sitecore.Data.Fields;
using Sitecore.Web.UI.WebControls;
using Synthesis.FieldTypes.Interfaces;
using Synthesis.Utility;

namespace Synthesis.FieldTypes
{
	public class HyperlinkField : FieldType, IHyperlinkField
	{
		public HyperlinkField(LazyField field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Checks if the field has a non-empty Href value
		/// </summary>
		public override bool HasValue
		{
			get
			{
				if (InnerField == null) return false;
				return !string.IsNullOrEmpty(Href);
			}
		}

		/// <summary>
		/// The CSS class of the link, if one was entered
		/// </summary>
		public virtual string CssClass
		{
			get { return ((LinkField)InnerField).Class; }
			set
			{
				SetFieldValue(delegate
				{
					((LinkField)InnerField).Class = value;
				});
			}
		}

		/// <summary>
		/// Gets the link URL. Handles internal links, external, media, etc types.
		/// </summary>
		public virtual string Href
		{
			get { return FieldUtility.GetGeneralLinkHref(InnerField); }
			set
			{
				SetFieldValue(delegate
					{
						LinkField innerField = InnerField;
						string[] splitValue = value.Split('?');
						innerField.Url = splitValue[0];

						if (splitValue.Length > 1)
							innerField.QueryString = splitValue[1];

						if (value.Contains("http")) innerField.LinkType = "external";
					});
			}
		}

		/// <summary>
		/// The target attribute of the link, if one was entered. Note that "_blank" is interpreted as rel="external" by default for XHTML compatibility.
		/// </summary>
		public virtual string Target
		{
			get { return ((LinkField)InnerField).Target; }
			set
			{
				SetFieldValue(delegate
				{
					((LinkField)InnerField).Target = value;
				});
			}
		}

		/// <summary>
		/// The text of the link, if one was entered. 
		/// </summary>
		public virtual string Text
		{
			get { return ((LinkField)InnerField).Text; }
			set
			{
				SetFieldValue(delegate
				{
					((LinkField)InnerField).Text = value;
				});
			}
		}

		/// <summary>
		/// The title attribute of the link, if one was entered
		/// </summary>
		public virtual string Title
		{
			get { return ((LinkField)InnerField).Title; }
			set
			{
				SetFieldValue(delegate
				{
					((LinkField)InnerField).Title = value;
				});
			}
		}

		/// <summary>
		/// Renders the field using a Sitecore FieldRenderer and returns the result
		/// </summary>
		public virtual string RenderedValue
		{
			get { return FieldRenderer.Render(InnerItem, InnerField.ID.ToString()); }
		}

		/// <summary>
		/// Shows the URL of the hyperlink
		/// </summary>
		public override string ToString()
		{
			return Href;
		}

		/// <summary>
		/// Gets the target Sitecore item of this link (if the link is internal)
		/// </summary>
		public IStandardTemplateItem TargetItem
		{
			get
			{
				if (HasValue)
					return ((LinkField)InnerField).TargetItem.AsStronglyTyped();

				return null;
			}
		}

		public LinkField ToLinkField()
		{
			return InnerField;
		}
	}
}
