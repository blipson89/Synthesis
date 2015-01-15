using Synthesis.Generation.Model;

namespace Synthesis.Generation
{
	public interface ITemplateCodeGenerator
	{
		void Generate(TemplateGenerationMetadata metadata);
	}
}
