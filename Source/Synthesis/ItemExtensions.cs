using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;
using Synthesis.Configuration;
using Synthesis.Initializers;

namespace Synthesis
{
	public static class ItemExtensions
	{
		/// <summary>
		/// Converts an Item into a strongly typed item
		/// </summary>
		/// <typeparam name="TTemplate">The custom item type (or interface type) you expect</typeparam>
		/// <returns>The converted item, or null if the conversion failed</returns>
		/// <remarks>If a conversion exception occurs, it can be found in the Sitecore log</remarks>
		public static TTemplate As<TTemplate>(this Item item)
			where TTemplate : class, IStandardTemplateItem
		{
			return item.AsStronglyTyped() as TTemplate;
		}

		/// <summary>
		/// Converts an Item into a strongly typed item
		/// </summary>
		/// <returns>The converted item, or null if the conversion failed</returns>
		/// <remarks>If a conversion exception occurs, it can be found in the Sitecore log</remarks>
		public static IStandardTemplateItem AsStronglyTyped(this Item item)
		{
			if (item == null)
				return null;

			ITemplateInitializer initializer = ProviderResolver.FindGlobalInitializer(item.TemplateID);

			return initializer.CreateInstance(item);
		}

		/// <summary>
		/// Converts an enumerable collection of Items into an enumerable collection of strongly typed items
		/// </summary>
		/// <typeparam name="TTemplate">The expected template of each item.</typeparam>
		/// <returns>Converted set of items. Non-matching templates or conversion errors will be excluded from the set.</returns>
		/// <remarks>If a conversion exception occurs, it can be found in the Sitecore log</remarks>
		public static IEnumerable<TTemplate> AsStronglyTypedCollectionOf<TTemplate>(this IEnumerable<Item> items)
			where TTemplate : class, IStandardTemplateItem
		{
			if (items == null) return Enumerable.Empty<TTemplate>();

			return items.Select(item => item.As<TTemplate>()).Where(result => result != null);
		}

		/// <summary>
		/// Converts an enumerable collection of Items into an enumerable collection of strongly typed items
		/// </summary>
		/// <returns>Converted set of items. Conversion errors will be excluded from the set.</returns>
		/// <remarks>If a conversion exception occurs, it can be found in the Sitecore log</remarks>
		public static IEnumerable<IStandardTemplateItem> AsStronglyTypedCollection(this IEnumerable<Item> items)
		{
			if (items == null) return Enumerable.Empty<IStandardTemplateItem>();

			return items.Select(item => item.AsStronglyTyped()).Where(result => result != null);
		}
	}
}
