using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using Synthesis.Configuration;
using Synthesis.Synchronization;
using System.Web.SessionState;
using Synthesis.Utility;

namespace Synthesis
{
	public class HttpHandler : IHttpHandler, IRequiresSessionState
	{
		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/html";

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

		private static void RenderLanding(HttpContext context)
		{
			context.Response.Write(WrapReport("Synthesis", CreateSyncSummary()));
		}

		private static string CreateSyncSummary()
		{
			var sb = new StringBuilder();

			// force getting the latest templates from sitecore
			ProviderResolver.Current.TemplateInputProvider.Refresh();

			var sync = ProviderResolver.CreateSyncEngine();

			var result = sync.AreTemplatesSynchronized();
			var count = result.Count(x => !x.IsSynchronized);

			if (result.AreTemplatesSynchronized)
				sb.Append("<p>Templates and model are currently synchronized. (Note: if Synthesis configuration file changes are made you should regenerate anyway)");
			else
			{
				sb.AppendFormat("<p>{0} template{1} not synchronized. ", count, count == 1 ? " is" : "s are");
				sb.Append("<a href=\"?synthesis-syncstatus=1\">Details</a>");
			}

			if (DebugUtility.IsDynamicDebugEnabled)
			{
				sb.Append(" | <a href=\"?synthesis-regenerate=1\">Regenerate Now</a>");
			}
			sb.Append("</p>");

			return sb.ToString();
		}

		private static void DoOnDemandSyncReport(HttpContext context)
		{
			var timer = new Stopwatch();
			timer.Start();

			// force getting the latest templates from sitecore
			ProviderResolver.Current.TemplateInputProvider.Refresh();

			var sync = ProviderResolver.CreateSyncEngine();

			var result = sync.AreTemplatesSynchronized();
			var sco = result.Where(x => x.Locations == SyncSource.Sitecore).ToList();
			var mo = result.Where(x => x.Locations == SyncSource.Model).ToList();
			var ma = result.Where(x => x.Locations == SyncSource.Both).ToList();
			var nonsyn = result.Where(x => !x.IsSynchronized).ToList();
			timer.Stop();

			var results = new StringBuilder();

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

			context.Response.Write(WrapReport("Synthesis Template Status", results.ToString()));

			context.Response.End();
		}

		private static void DoOnDemandRegenerate(HttpContext context)
		{
			var timer = new Stopwatch();
			timer.Start();
			ProviderResolver.Current.TemplateInputProvider.Refresh();
			ProviderResolver.CreateGenerator().GenerateToDisk();
			timer.Stop();

			string result = string.Format("<p>Generation complete in {0} ms. You will want to rebuild to pick up the changes.</p>", timer.ElapsedMilliseconds);
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
			sb.AppendLine("h1, p, td, th, li { font-family: Helvetica, Arial, Sans-serif; list-style-type: none; }");
			sb.AppendLine("#container { margin: 0 auto; width: 650px; border: 1px solid gray; }");
			sb.AppendLine("h1 { padding: 10px; margin: 0; border-bottom: 1px solid gray; filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#EEE', endColorstr='#CCC'); /* for IE */ background: -webkit-gradient(linear, left top, left bottom, from(#EEE), to(#CCC)); /* for webkit browsers */ background: -moz-linear-gradient(top,  #EEE,  #CCC); /* for firefox 3.6+ */ }");
			sb.AppendLine("table, p, h2, ul { margin: 10px; padding: 0 }");
			sb.AppendLine("li { margin: 0px; padding: 3px 0; }");
			sb.AppendLine("th { text-align: left; }");
			sb.AppendLine("th.flag { width: 75px; }");
			sb.AppendLine("tr:nth-child(even) { background-color:#eee; }");
			sb.AppendLine("td { font-size: 0.7em; }");
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
