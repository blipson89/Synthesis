using System;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Testing
{
	public class TestFileField : TestFieldType, IFileField
	{
		public TestFileField(string url)
		{
			Url = url;
			MediaItemId = ID.Null;
		}

		/// <summary>
		///     Gets the URL to the media item. If HasValue is false returns an empty string.
		/// </summary>
		public string Url { get; private set; }

		/// <summary>
		///     Gets the ID of the media item the mediafield references
		/// </summary>
		public ID MediaItemId { get; set; }

		/// <summary>
		///     The MediaItem that is the target of the file field
		/// </summary>
		public MediaItem MediaItem
		{
			get { throw new NotImplementedException("Test field types cannot access Sitecore classes."); }
		}

		/// <summary>
		///     Gets a stream of the binary data in the file field. Make sure to dispose of the object when done!
		/// </summary>
		public MediaStream MediaStream
		{
			get { throw new NotImplementedException("Test field types cannot access Sitecore classes."); }
		}

		/// <summary>
		///     Checks if the media field has a reference value. Does not validate that the referenced ID actually exists.
		/// </summary>
		public override bool HasValue
		{
			get { return string.IsNullOrEmpty(Url); }
		}
	}
}