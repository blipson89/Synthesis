
using System.Diagnostics.CodeAnalysis;
namespace Synthesis.Synchronization
{
	/// <summary>
	/// The resulting metadata around a template to model comparison result
	/// </summary>
	public class TemplateComparisonResult
	{
		public TemplateComparisonResult(string templateId, string sitecoreName, string modelName, string sitecoreSignature, string modelSignature)
		{
			TemplateID = templateId;

			SitecoreTemplateName = sitecoreName;
			ModelTypeName = modelName;
			
			SitecoreSignature = sitecoreSignature;
			ModelSignature = modelSignature;
		}

		/// <summary>
		/// Name of the template in Sitecore
		/// </summary>
		public string SitecoreTemplateName { get; private set; }

		/// <summary>
		/// Type of the template in the domain model
		/// </summary>
		public string ModelTypeName { get; private set; }

		/// <summary>
		/// The template's GUID
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID", Justification="Coherent with Sitecore conventions")]
		public string TemplateID { get; private set; }

		/// <summary>
		/// Version signature calculated based on Sitecore state
		/// </summary>
		public string SitecoreSignature { get; private set; }

		/// <summary>
		/// Version signature from the model type's attribute
		/// </summary>
		public string ModelSignature { get; private set; }

		/// <summary>
		/// Where the compared template is present
		/// </summary>
		public SyncSource Locations
		{
			get
			{
				if (!string.IsNullOrEmpty(SitecoreSignature) && !string.IsNullOrEmpty(ModelSignature))
					return SyncSource.Both;
				if (!string.IsNullOrEmpty(SitecoreSignature))
					return SyncSource.Sitecore;
				if (!string.IsNullOrEmpty(ModelSignature))
					return SyncSource.Model;

				return SyncSource.Both; // this should never occur, but must be here for completeness
			}
		}

		/// <summary>
		/// Checks if the template is synchronized between the model and Sitecore
		/// </summary>
		public bool IsSynchronized { get { return SitecoreSignature == ModelSignature; } }

		public override string ToString()
		{
			if (Locations == SyncSource.Both)
				return string.Format("{0}, represented by {1}, found in both, {2}synchronized", SitecoreTemplateName, ModelTypeName, (IsSynchronized) ? string.Empty : "not ");
			
			if (Locations == SyncSource.Sitecore)
				return string.Format("{0}, found only in Sitecore", SitecoreTemplateName);
			
			if (Locations == SyncSource.Model)
				return string.Format("{0}, found only in Model", ModelTypeName);
			
			return string.Empty;
		}
	}
}
