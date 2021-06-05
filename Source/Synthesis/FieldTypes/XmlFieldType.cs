using Sitecore.Data.Fields;

namespace Synthesis.FieldTypes
{
	public abstract class XmlFieldType : FieldType
	{
		protected XmlFieldType(LazyField innerField, string searchValue) : base(innerField, searchValue)
		{
		}

		/// <summary>
		/// Gets the value of the attribute named <paramref name="attribute"/>
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		protected string GetAttribute(string attribute) => ((XmlField) InnerField).GetAttribute(attribute);
	}
}
