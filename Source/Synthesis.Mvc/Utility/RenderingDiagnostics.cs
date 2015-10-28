using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Web;
using System.Web.UI;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Presentation;

namespace Synthesis.Mvc.Utility
{
	public class RenderingDiagnostics : IDisposable
	{
		readonly HtmlTextWriter _writer;
		readonly string _renderingName;
		readonly bool _cacheable;
		readonly bool _varyByData;
		readonly bool _varyByDevice;
		readonly bool _varyByLogin;
		readonly bool _varyByParm;
		readonly bool _varyByQueryString;
		readonly bool _varyByUser;
		readonly TimeSpan _timeout;
		readonly string _varyByCustom;
		readonly Stopwatch _timer;

		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "It's what Sitecore uses")]
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Parm", Justification = "It's what Sitecore uses")]
		[SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Intentional extension point")]
		public RenderingDiagnostics(HtmlTextWriter writer, string renderingName, RenderingCachingDefinition cachingDefinition)
		{
			Assert.IsNotNull(writer, "HtmlTextWriter cannot be null");
			Assert.IsNotNull(renderingName, "Rendering name cannot be null");

			_writer = writer;
			_renderingName = renderingName;
			_cacheable = cachingDefinition.Cacheable;
			_varyByData = cachingDefinition.VaryByData;
			_varyByDevice = cachingDefinition.VaryByDevice;
			_varyByLogin = cachingDefinition.VaryByLogin;
			_varyByParm = cachingDefinition.VaryByParameters;
			_varyByQueryString = cachingDefinition.VaryByQueryString;
			_varyByUser = cachingDefinition.VaryByUser;
			_varyByCustom = cachingDefinition.CacheKey;
			_timeout = cachingDefinition.Timeout;

			_timer = new Stopwatch();

			RenderingStartDiagnostics();
		}

		protected virtual void RenderingStartDiagnostics()
		{
			//<!-- Begin Rendering "~/bar/Foo.cshtml" -->
			//<!-- Rendering was output cached at {datetime}, VaryByData, CachingID = "loremipsum" -->

			var comment = new StringBuilder();

			comment.AppendFormat("<!-- Begin Rendering {0} -->\n", _renderingName);
			if (!_cacheable)
			{
				_writer.Write(comment.ToString());
				return;
			}

			comment.AppendFormat("<!-- Rendering was output cached at {0}", DateTime.Now);
			if (_timeout.TotalSeconds > 0)
				comment.AppendFormat(", Cache timeout {0} sec", _timeout.TotalSeconds);
			if (_varyByData)
				comment.Append(", VaryByData");
			if (_varyByDevice)
				comment.Append(", VaryByDevice");
			if (_varyByLogin)
				comment.Append(", VaryByLogin");
			if (_varyByParm)
				comment.Append(", VaryByParm");
			if (_varyByQueryString)
				comment.Append(", VaryByQueryString");
			if (_varyByUser)
				comment.Append(", VaryByUser");
			if (!string.IsNullOrEmpty(_varyByCustom))
				comment.AppendFormat(", VaryByCustom=\"{0}\"", HttpUtility.HtmlEncode(_varyByCustom));

			comment.Append(" -->");

			_writer.Write(comment.ToString());

			_timer.Start();
		}

		protected virtual void RenderingEndDiagnostics()
		{
			// <!-- End Rendering "~/bar/Foo.ascx" -->
			_timer.Stop();
			_writer.Write("<!-- End Rendering {0}, render took {1:N1}ms{2} -->", _renderingName, _timer.Elapsed.TotalMilliseconds, _cacheable ? " (timing is without output cache as this text is cached)" : string.Empty);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				RenderingEndDiagnostics();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
