using Synthesis.FieldTypes;

namespace Synthesis.Tests
{
	//[RepresentsSitecoreTemplate("{76036F5E-ABCE-46D1-AF0A-4143F9B557AA}", "DN8cOiiO0ckeD/NPjd9Q8nJuPSk=")]
	internal interface IFakeTemplateItem
	{
		BooleanField YesOrNo { get; }
		DateTimeField Timestamp { get; }
		FileField File { get; }
		HyperlinkField Link { get; }
		ImageField TerriblePicture { get; }
		IntegerField DaysTillChristmas { get; }
		ItemReferenceListField RelatedFolders { get; }
		NumericField AccountBalance { get; }
		ItemReferenceField RelatedFolder { get; }
		TextField Title { get; }
		WordDocumentField BadlyDoneHtml { get; }
	}
}
