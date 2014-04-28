namespace Synthesis.ContentSearch
{
	public class DocumentMappingResult<TItem>
	{
		public DocumentMappingResult(TItem result, bool success)
		{
			Document = result;
			MappedSuccessfully = success;
		}

		public TItem Document { get; private set; }
		public bool MappedSuccessfully { get; private set; }
	}
}
