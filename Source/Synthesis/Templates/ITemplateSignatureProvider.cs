namespace Synthesis.Templates
{
	/// <summary>
	/// Provides a unique signature for a template used for comparing versions
	/// </summary>
	public interface ITemplateSignatureProvider
	{
		string GenerateTemplateSignature(ITemplateInfo templateItem);
	}
}
