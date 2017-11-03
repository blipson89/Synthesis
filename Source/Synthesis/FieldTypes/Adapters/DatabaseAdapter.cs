using System.Linq;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Globalization;
using Version = Sitecore.Data.Version;

namespace Synthesis.FieldTypes.Adapters
{
	public class DatabaseAdapter : IDatabaseAdapter
	{
		private readonly Database _database;

		public DatabaseAdapter(Database database)
		{
			_database = database;
		}

		public string Name => _database.Name;

		public Database InnerDatabase => _database;

		public IStandardTemplateItem GetItem(ID itemId)
		{
			return _database.GetItem(itemId).AsStronglyTyped();
		}

		public IStandardTemplateItem GetItem(ID itemId, Language language)
		{
			return _database.GetItem(itemId, language).AsStronglyTyped();
		}

		public IStandardTemplateItem GetItem(ID itemId, Language language, Version version)
		{
			return _database.GetItem(itemId, language, version).AsStronglyTyped();
		}

		public IStandardTemplateItem GetItem(string path)
		{
			return _database.GetItem(path).AsStronglyTyped();
		}

		public IStandardTemplateItem GetItem(string path, Language language)
		{
			return _database.GetItem(path, language).AsStronglyTyped();
		}

		public IStandardTemplateItem GetItem(string path, Language language, Version version)
		{
			return _database.GetItem(path, language, version).AsStronglyTyped();
		}

		public IStandardTemplateItem GetItem(DataUri uri)
		{
			return _database.GetItem(uri).AsStronglyTyped();
		}

		public LanguageCollection GetLanguages()
		{
			return _database.GetLanguages();
		}

		public IStandardTemplateItem GetRootItem()
		{
			return _database.GetRootItem().AsStronglyTyped();
		}

		public IStandardTemplateItem GetRootItem(Language language)
		{
			return _database.GetRootItem(language).AsStronglyTyped();
		}

		public IStandardTemplateItem[] SelectItems(string query)
		{
			return _database.SelectItems(query).AsStronglyTypedCollection().ToArray();
		}

		public IStandardTemplateItem SelectSingleItem(string query)
		{
			return _database.SelectSingleItem(query).AsStronglyTyped();
		}
	}
}