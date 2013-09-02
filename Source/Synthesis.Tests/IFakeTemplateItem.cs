using Synthesis.FieldTypes.Interfaces;

namespace Synthesis.Tests
{
	//[RepresentsSitecoreTemplate("{76036F5E-ABCE-46D1-AF0A-4143F9B557AA}", "DN8cOiiO0ckeD/NPjd9Q8nJuPSk=")]
	internal interface IFakeTemplateItem
	{
		IBooleanField YesOrNo { get; }
		IDateTimeField Timestamp { get; }
		IFileField File { get; }
		IHyperlinkField Link { get; }
		IImageField TerriblePicture { get; }
		IIntegerField DaysTillChristmas { get; }
		IItemReferenceListField RelatedFolders { get; }
		INumericField AccountBalance { get; }
		IItemReferenceField RelatedFolder { get; }
		ITextField Title { get; }
	}
}
