using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.WordOCX;

namespace Synthesis.FieldTypes
{
	public class WordDocumentField : FieldType
	{
		public WordDocumentField(Lazy<Field> field, string indexValue) : base(field, indexValue) { }

		/// <summary>
		/// Gets the URL to download a word version (.docx) of the field value
		/// </summary>
		public virtual string Url
		{
			get 
			{
				var field = GetField();

				if(field == null) return null;

				var parameters = new Dictionary<string, string>();
				parameters["db"] = InnerItem.Database.Name;
				parameters["blobId"] = field.BlobId.ToString();
				var wordManager = new WordOCXUrlManager(parameters);
				return wordManager.GetDownloadLink();
			}
		}

		/// <summary>
		/// Gets the plain text version of the word document, or an empty string if the document is invalid
		/// </summary>
		public virtual string TextValue
		{
			get
			{
				var field = GetField();

				if (field == null) return string.Empty;

				return field.PlainText;
			}
		}

		/// <summary>
		/// Gets the HTML version of the word document, or an empty string if the document is invalid. Does not include the CSS.
		/// </summary>
		public virtual string HtmlValue
		{
			get 
			{
				var field = GetField();

				if (field == null) return string.Empty;

				return field.Html;
			}
		}

		/// <summary>
		/// Gets the CSS styles to go with the HTML version of the word document, or an empty string if the document is invalid
		/// </summary>
		/// <remarks>You wouldn't actually render these to a page, would you? Would you?</remarks>
		public string HtmlStyles
		{
			get
			{
				var field = GetField();

				if (field == null) return string.Empty;

				return field.Styles;
			}
		}

		/// <summary>
		/// Checks if a valid word document is attached to the field
		/// </summary>
		public override bool HasValue
		{
			get
			{
				if (InnerField == null) return false; 
				return GetField() != null;
			}
		}

		private Sitecore.Data.Fields.WordDocumentField GetField()
		{
			var field = (Sitecore.Data.Fields.WordDocumentField)InnerField;

			if (field == null || field.BlobId == ID.Null || Regex.IsMatch(field.PlainText, "^\\s*$")) return null;

			return field;
		}
	}
}
