using Synthesis.Generation.Model;

namespace Synthesis.Generation
{
	public interface ITemplateCodeGenerator
	{
		void Generate(TemplateGenerationData metadata);
	}
}
