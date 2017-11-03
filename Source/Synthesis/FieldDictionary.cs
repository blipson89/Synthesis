using Synthesis.FieldTypes;

namespace Synthesis
{
	/// <summary>
	/// Provides indexed access to Synthesis item fields by name
	/// </summary>
	public class FieldDictionary
	{
		private readonly IStandardTemplateItem _item;

		internal FieldDictionary(IStandardTemplateItem item)
		{
			_item = item;
		}

		public FieldType this[string fieldName]
		{
			get
			{
				var field = _item.InnerItem.Fields[fieldName];

				return field?.AsStronglyTyped();
			}
		}
	}
}
