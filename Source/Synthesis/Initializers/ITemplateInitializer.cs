using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Synthesis.Initializers
{
	/// <summary>
	/// Represents the contract for a template initializer (a proxy class that allows fast, reflection-less creation of model objects)
	/// </summary>
	public interface ITemplateInitializer
	{
		IStandardTemplateItem CreateInstance(Item innerItem);
		IStandardTemplateItem CreateInstanceFromSearch(IDictionary<string, string> searchFields);
		ID InitializesTemplateId { get; }
	}
}
