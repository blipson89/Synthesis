using System;
using Synthesis.ContentSearch;

namespace Synthesis.Tests.Fixtures.ContentSearch
{
	internal class InitializerForcer : IDisposable
	{
		public InitializerForcer(ITemplateInitializer initializer)
		{
			SynthesisDocumentTypeMapper.OverrideInitializer(initializer);
		}

		public void Dispose()
		{
			SynthesisDocumentTypeMapper.RemoveOverrideInitializer();
		}
	}
}
