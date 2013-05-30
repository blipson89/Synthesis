using Sitecore.Data.Items;

namespace Synthesis
{
#pragma warning disable 0618 // disable the warning about InnerItem here - it's cool

	/// <summary>
	/// Provides a means to put an item in editing mode.
	/// </summary>
	/// <remarks>
	///  The item is enters editing mode when this object is created and leaves editing
	///  mode when the object is disposed.
	///  This provides an easy to use mechanism for editing items.
	/// </remarks>
	public class SynthesisEditContext : EditContext
	{
		public SynthesisEditContext(IStandardTemplateItem item) : base(item.InnerItem)
		{

		}
	}

#pragma warning restore 0618
}
