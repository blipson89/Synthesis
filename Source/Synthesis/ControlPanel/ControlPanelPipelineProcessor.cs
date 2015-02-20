using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Security.Authentication;
using Sitecore.SecurityModel;
using Sitecore.StringExtensions;
using Synthesis.Configuration;
using Synthesis.Synchronization;
using Synthesis.Utility;

namespace Synthesis.ControlPanel
{
	/// <summary>
	/// This is a httpRequestBegin pipeline processor that is effectively a sitecore-integrated HTTP handler.
	/// It renders the Unicorn control panel UI if the current URL matches the activationUrl.
	/// </summary>
	public class ControlPanelPipelineProcessor : HttpRequestProcessor
	{
		private readonly string _activationUrl;

		public ControlPanelPipelineProcessor(string activationUrl)
		{
			_activationUrl = activationUrl;
		}


		public override void Process(HttpRequestArgs args)
		{
			if (string.IsNullOrWhiteSpace(_activationUrl)) return;

			if (args.Context.Request.RawUrl.StartsWith(_activationUrl, StringComparison.OrdinalIgnoreCase))
			{
				ProcessRequest(args.Context);
				args.Context.Response.End();
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			context.Server.ScriptTimeout = 86400;
			context.Response.ContentType = "text/html";

			if (!IsAllowed())
			{
				context.Response.Write(WrapReport("Access Denied", "You may need to sign in to Sitecore as an administrator."));
			}
			else
			{
				// this securitydisabler allows the control panel to execute unfettered when debug compilation is enabled but you are not signed into Sitecore
				using (new SecurityDisabler())
				{
					if (context.Request.QueryString["synthesis-regenerate"] != null && DebugUtility.IsDynamicDebugEnabled)
					{
						DoOnDemandRegenerate(context);
					}

					if (context.Request.QueryString["synthesis-syncstatus"] != null)
					{
						DoOnDemandSyncReport(context);
					}

					RenderLanding(context);
				}
			}
		}

		protected virtual bool IsAllowed()
		{
			var user = AuthenticationManager.GetActiveUser();

			if (user.IsAdministrator)
			{
				return true;
			}

			// if dynamic debug compilation is enabled, you can use it without auth (eg local dev)
			if (HttpContext.Current.IsDebuggingEnabled)
				return true;

			return false;
		}

		private static void RenderLanding(HttpContext context)
		{
			context.Response.Write(WrapReport("Synthesis", CreateSyncSummary()));
		}

		private static string CreateSyncSummary()
		{
			var sb = new StringBuilder();

			var configurations = ProviderResolver.GetConfigurations();

			if (configurations.Any())
			{
				foreach (var configuration in configurations)
				{
					var configName = configuration.Name.IsNullOrEmpty() ? "Unnamed Configuration" : configuration.Name;

					// force getting the latest templates from sitecore
					configuration.TemplateInputProvider.Refresh();

					var sync = configuration.CreateSyncEngine();

					var result = sync.AreTemplatesSynchronized();
					var count = result.Count(x => !x.IsSynchronized);

					if (result.AreTemplatesSynchronized)
						sb.AppendFormat("<p><strong>{0}</strong>: Templates and model are currently synchronized.</p>", configName);
					else
					{
						sb.AppendFormat("<p><strong>{0}</strong>: {1} template{2} not synchronized. ", configName, count,
							count == 1 ? " is" : "s are");
						sb.Append("<a href=\"?synthesis-syncstatus=1\">Details</a>");
					}
				}

				sb.Append("<p>Note: if Synthesis configuration file changes are made you should force a regenerate</p>");

				if (DebugUtility.IsDynamicDebugEnabled)
				{
					sb.Append("<p><a href=\"?synthesis-regenerate=1\">Regenerate Now</a></p>");
				}
				else
				{
					sb.Append("<p>(enable debug compilation if you want to regenerate)</p>");
				}
			}
			else
			{
				sb.Append(@"<p><em>Synthesis currently has no model configurations registered.</em></p> 
<p>This probably means you need to enable the <code>RegisterDefaultConfiguration</code> processor in the <code>initialize</code> pipeline, 
or that you need to register your own configurations in separate initialize pipeline processors.</p>
<p>Synthesis cannot run any generation or syncing until a configuration is registered</p>.");
			}

			return sb.ToString();
		}

		private static void DoOnDemandSyncReport(HttpContext context)
		{
			var configurations = ProviderResolver.GetConfigurations();

			var results = new StringBuilder();

			foreach (var configuration in configurations)
			{
				var timer = new Stopwatch();
				timer.Start();

				var configName = configuration.Name.IsNullOrEmpty() ? "Unnamed Configuration" : configuration.Name;

				// force getting the latest templates from sitecore
				configuration.TemplateInputProvider.Refresh();

				var sync = configuration.CreateSyncEngine();

				var result = sync.AreTemplatesSynchronized();
				var sco = result.Where(x => x.Locations == SyncSource.Sitecore).ToList();
				var mo = result.Where(x => x.Locations == SyncSource.Model).ToList();
				var ma = result.Where(x => x.Locations == SyncSource.Both).ToList();
				var nonsyn = result.Where(x => !x.IsSynchronized).ToList();
				timer.Stop();

				results.AppendFormat("<h2>{0}</h2>", configName);

				results.AppendLine("<ul>");

				results.AppendLine("<li>Synchronized: <strong>" + result.AreTemplatesSynchronized.ToString() + "</strong></li>");
				results.AppendLine("<li>Total Items: <strong>" + result.Count + "</strong></li>");
				results.AppendLine("<li>Total Not Synchronized: <strong>" + nonsyn.Count + "</strong></li>");
				results.AppendLine("<li>Total Sitecore Only: <strong>" + sco.Count + "</strong></li>");
				results.AppendLine("<li>Total Model Only: <strong>" + mo.Count + "</strong></li>");
				results.AppendLine("<li>Total in Both: <strong>" + ma.Count + "</strong></li>");
				results.AppendLine("<li>Sync time taken: <strong>" + timer.ElapsedMilliseconds + " ms</strong></li>");

				results.AppendLine("</ul>");

				results.AppendLine("<table cellpadding=\"3\" cellspacing=\"1\">");
				results.AppendLine("<thead><tr>");
				results.AppendLine("<th>Template</th><th class=\"flag\">Sitecore</th><th class=\"flag\">Model</th><th class=\"flag\">Sync</th>");
				results.AppendLine("</tr></thead>");

				results.AppendLine("<tbody>");
				foreach (var item in result.OrderBy(x => x.SitecoreTemplateName ?? x.ModelTypeName))
				{
					results.AppendLine("<tr>");

					results.AppendFormat("<td>{0}</td>", item.SitecoreTemplateName ?? item.ModelTypeName);
					results.AppendFormat("<td style=\"background-color: {0}\">&nbsp;</td>", (item.Locations == SyncSource.Sitecore || item.Locations == SyncSource.Both) ? "green" : "red");
					results.AppendFormat("<td style=\"background-color: {0}\">&nbsp;</td>", (item.Locations == SyncSource.Model || item.Locations == SyncSource.Both) ? "green" : "red");
					results.AppendFormat("<td style=\"background-color: {0}\">&nbsp;</td>", (item.IsSynchronized) ? "green" : "red");

					results.AppendLine("</tr>");
				}
				results.AppendLine("</tbody></table>");
			}
			
			context.Response.Write(WrapReport("Synthesis Template Status", results.ToString()));

			context.Response.End();
		}

		private static void DoOnDemandRegenerate(HttpContext context)
		{
			var timer = new Stopwatch();
			timer.Start();

			var configurations = ProviderResolver.GetConfigurations();
			string result = string.Empty;

			foreach (var configuration in configurations)
			{
				var configName = configuration.Name.IsNullOrEmpty() ? "Unnamed Configuration" : configuration.Name;
				configuration.TemplateInputProvider.Refresh();
				var metadata = configuration.CreateMetadataGenerator().GenerateMetadata();
				configuration.CreateCodeGenerator().Generate(metadata);
				result += "<p>Regenerated <strong>{0}</strong></p>".FormatWith(configName);
			}
			timer.Stop();

			result += string.Format("<p>Generation complete in {0} ms. You will want to rebuild to pick up the changes.</p>", timer.ElapsedMilliseconds);
			context.Response.Write(WrapReport("Regenerating Model", result));

			context.Response.End();
		}

		private static string WrapReport(string title, string contents)
		{
			var sb = new StringBuilder();

			sb.AppendLine("<!DOCTYPE html>");
			sb.AppendLine("<html>");

			sb.AppendLine("<head>");
			sb.AppendLine("<style type=\"text/css\">");
			sb.AppendLine("h1, h2, p, td, th, li { font-family: Helvetica, Arial, Sans-serif; list-style-type: none; }");
			sb.AppendLine("#container { margin: 0 auto; width: 650px; border: 1px solid gray; }");
			sb.AppendLine("h1 { padding: 10px; margin: 0; border-bottom: 1px solid gray; filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#EEE', endColorstr='#CCC'); /* for IE */ background: -webkit-gradient(linear, left top, left bottom, from(#EEE), to(#CCC)); /* for webkit browsers */ background: -moz-linear-gradient(top,  #EEE,  #CCC); /* for firefox 3.6+ */ }");
			sb.AppendLine("table, p, h2, ul { margin: 10px; padding: 0 }");
			sb.AppendLine("li { margin: 0px; padding: 3px 0; }");
			sb.AppendLine("th { text-align: left; }");
			sb.AppendLine("th.flag { width: 75px; }");
			sb.AppendLine("tr:nth-child(even) { background-color:#eee; }");
			sb.AppendLine("td { font-size: 0.7em; }");
			sb.AppendLine("code { font-family: Consolas, monospace; }");
			sb.AppendLine("</style>");
			sb.AppendLine("<title>" + HttpUtility.HtmlEncode(title) + "</title>");
			sb.AppendLine("</head>");

			sb.AppendLine("<body>");
			sb.AppendLine("<div id=\"container\">");
			sb.AppendLine("<h1>" + HttpUtility.HtmlEncode(title) + "</h1>");
			sb.Append(contents);
			sb.AppendLine("</div>");
			sb.AppendLine("</body>");
			sb.AppendLine("</html>");

			return sb.ToString();
		}


	}
}
