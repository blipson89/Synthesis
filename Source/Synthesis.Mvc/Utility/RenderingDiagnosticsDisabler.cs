using System;

namespace Synthesis.Mvc.Utility
{
	public class RenderingDiagnosticsDisabler : IDisposable
	{
		private readonly bool _originalValue;
		public RenderingDiagnosticsDisabler()
		{
			_originalValue = RenderingDiagnostics.DiagnosticsEnabledForThisRequest;
			RenderingDiagnostics.DiagnosticsEnabledForThisRequest = false;
		}

		public void Dispose()
		{
			RenderingDiagnostics.DiagnosticsEnabledForThisRequest = _originalValue;
		}
	}
}
