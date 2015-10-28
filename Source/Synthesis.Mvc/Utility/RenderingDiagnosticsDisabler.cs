using System;
using Synthesis.Mvc.Pipelines.GetRenderer;

namespace Synthesis.Mvc.Utility
{
	public class RenderingDiagnosticsDisabler : IDisposable
	{
		private readonly bool _originalValue;
		public RenderingDiagnosticsDisabler()
		{
			_originalValue = RenderingDiagnosticsInjector.DiagnosticsEnabledForThisRequest;
			RenderingDiagnosticsInjector.DiagnosticsEnabledForThisRequest = false;
		}

		public void Dispose()
		{
			RenderingDiagnosticsInjector.DiagnosticsEnabledForThisRequest = _originalValue;
		}
	}
}
