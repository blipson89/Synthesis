using System;
using System.Text.RegularExpressions;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Resources.Media;

namespace Synthesis.Utility
{
	public static class FieldUtility
	{
		/// <summary>
		/// Gets the correct URL for a media item according to SDN as of SC 6.4
		/// </summary>
		public static string GetMediaUrl(MediaItem item)
		{
			Assert.ArgumentNotNull(item, "item");

			// If Media.AlwaysIncludeServerUrl is set, it should override the LinkManager setting for
            // media links.
		    bool includeServerUrl = LinkManager.Provider.AlwaysIncludeServerUrl;
		    string mediaAlwaysIncludeServerUrl = Sitecore.Configuration.Settings.GetSetting("Media.AlwaysIncludeServerUrl", string.Empty);
		    if (!string.IsNullOrEmpty(mediaAlwaysIncludeServerUrl))
		    {
		        includeServerUrl = (mediaAlwaysIncludeServerUrl.ToLower() == "true");
		    }

			// the conditional here prevents URLs like /http://foo/bar from being generated if AlwaysIncludeServerUrl is enabled
			// thanks to Dave Peterson for finding this.
			return includeServerUrl ? MediaManager.GetMediaUrl(item) : StringUtil.EnsurePrefix('/', HttpUtility.UrlPathEncode(MediaManager.GetMediaUrl(item)));
		}

		/// <summary>
		/// Gets the correct URL for a general link field according to SDN as of SC 6.4
		/// </summary>
		public static string GetGeneralLinkHref(LinkField field)
		{
			Assert.ArgumentNotNull(field, "field");

			// so Sitecore, why isn't this encapsulated into the LinkField class again?
			string url;

			switch (field.LinkType)
			{
				case "internal":
					// catch internal links with external HTTP URLs
					if (Regex.IsMatch(field.Url, "^https?://"))
						return field.Url;

					if (field.TargetItem == null) return field.Url ?? string.Empty;
					url = LinkManager.GetItemUrl(field.TargetItem);
					break;
				case "external":
				case "mailto":
				case "anchor":
				case "javascript":
					url = field.Url;
					break;
				case "media":
					Item target = field.TargetItem;

					if (target == null) return string.Empty;

					url = GetMediaUrl(target);
					break;
				case "":
					return string.Empty;
				default:
					string message = String.Format("Unknown link type {0} in {1}", field.LinkType, field.InnerField.Item.Paths.FullPath);
					Log.Warn(message, typeof(FieldUtility));

					return string.Empty;
			}

			if (!string.IsNullOrEmpty(field.QueryString))
			{
				url += "?" + field.QueryString.TrimStart('?');
			}

			return url;
		}

		/// <summary>
		/// Takes a piece of content that may contain dynamic links (e.g. rich text content) and expands them from dynamic link format (i.e. /~/link/dasfsdgsdfsdf) to their appropriate
		/// friendly URL. This is normally handled by FieldRenderer automatically, but this brings that to places where FieldRender isn't necessary or desirable.
		/// </summary>
		public static string ExpandDynamicLinks(string fieldContent)
		{
			string containsFriendlyLinks = LinkManager.ExpandDynamicLinks(fieldContent, Sitecore.Configuration.Settings.Rendering.SiteResolving);
			string mediaPrefix = string.Empty;
			foreach (string currentPrefix in MediaManager.Provider.Config.MediaPrefixes)
			{
				if (containsFriendlyLinks.IndexOf(currentPrefix, 0, StringComparison.Ordinal) > 0)
				{
					mediaPrefix = currentPrefix;
					break;
				}
			}

			if (mediaPrefix.Length > 0) containsFriendlyLinks = Regex.Replace(containsFriendlyLinks, "([^/])" + mediaPrefix, "$1/" + mediaPrefix);

			return containsFriendlyLinks;
		}
	}
}
