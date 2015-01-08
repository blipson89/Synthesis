using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Synthesis.Templates
{
	/// <summary>
	/// Provides a signature of a template based on the SHA-1 hash of its path, ID and field names, IDs, and types
	/// This should be a unique signature for all modifications that would affect the standard domain model generation
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sha", Justification="It's a hash algorithm name")]
	public class Sha1TemplateSignatureProvider : ITemplateSignatureProvider
	{
		public virtual string GenerateTemplateSignature(ITemplateInfo templateItem)
		{
			string signatureText = GenerateTextSignature(templateItem);
			return HashTextSignature(signatureText);
		}

		protected virtual string GenerateTextSignature(ITemplateInfo templateItem)
		{
			List<string> keyElements = new List<string>();

			keyElements.Add(templateItem.FullPath); // full path will cover us for namespace changes
			keyElements.Add(templateItem.TemplateId.ToString()); // ID is used for creating new items and syncing

			foreach (var field in templateItem.OwnFields)
			{
				keyElements.Add(field.Name); // name determines property names
				keyElements.Add(field.Id.ToString()); // ID determines internal property lookups for speed
				keyElements.Add(field.Type); // type determines property Type
			}

			keyElements.Sort(); // sorting these gives us a standard order, so that merely reordering fields doesn't potentially invalidate the signature

			return string.Join("|", keyElements.ToArray());
		}

		protected virtual string HashTextSignature(string rawSignature)
		{
			byte[] hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(rawSignature));

			return Convert.ToBase64String(hash);
		}
	}
}
