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
			return StringUtil.EnsurePrefix('/', MediaManager.GetMediaUrl(item));
		}

		/// <summary>
		/// Gets the correct URL for a general link field according to SDN as of SC 6.4
		/// </summary>
		public static string GetGeneralLinkHref(LinkField field)
		{
			// so Sitecore, why isn't this encapsulated into the LinkField class again?
			string url = string.Empty;

			switch (field.LinkType)
			{
				case "internal":
					// catch internal links with external HTTP URLs
					if (Regex.IsMatch(field.Url, "^https?://"))
						return field.Url;

					if (field.TargetItem == null) return string.Empty;
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
			string containsFriendlyLinks = LinkManager.ExpandDynamicLinks(fieldContent, global::Sitecore.Configuration.Settings.Rendering.SiteResolving);
			string mediaPrefix = string.Empty;
			foreach (string currentPrefix in MediaManager.Provider.Config.MediaPrefixes)
			{
				if (containsFriendlyLinks.IndexOf(currentPrefix, 0) > 0)
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
