using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.StringExtensions;
using Synthesis.Configuration;
using Synthesis.Utility;

namespace Synthesis
{
	internal static class Initializers
	{
		private static readonly ReadOnlyDictionary<ID, ITemplateInitializer> InitializerCache;

		[SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification="Too much logic to inline")]
		static Initializers()
		{
			var timer = new Stopwatch();
			timer.Start();

			var initializers = new Dictionary<ID, ITemplateInitializer>();
	
			var initializerTypes = ProviderResolver.Current.TypeListProvider.CreateTypeList().ImplementingInterface(typeof(ITemplateInitializer));

			foreach (var initializer in initializerTypes)
			{
				var instance = (ITemplateInitializer)Activator.CreateInstance(initializer);

				if (initializers.ContainsKey(instance.InitializesTemplateId))
				{
					throw new InvalidOperationException("Synthesis: Multiple initializers were found for template {0} ({1}, {2}).".FormatWith(instance.InitializesTemplateId, initializers[instance.InitializesTemplateId].GetType().FullName, instance.GetType().FullName));
				}

				// ignore test and standard template initializers
				if (instance.InitializesTemplateId == ID.Null) continue;

				initializers.Add(instance.InitializesTemplateId, instance);
			}

			InitializerCache = new ReadOnlyDictionary<ID, ITemplateInitializer>(initializers);

			timer.Stop();
			Log.Info("Synthesis: Initialized template initializer cache (" + InitializerCache.Count + " templates) in " + timer.ElapsedMilliseconds + " ms", InitializerCache);
		}

		public static ITemplateInitializer GetInitializer(ID templateId)
		{
			ITemplateInitializer initializer;
			if (InitializerCache.TryGetValue(templateId, out initializer)) return initializer;

			return new StandardTemplateInitializer();
		}

		private class StandardTemplateInitializer : ITemplateInitializer
		{
			public IStandardTemplateItem CreateInstance(Item innerItem)
			{
				return new StandardTemplateItem(innerItem);
			}

			public IStandardTemplateItem CreateInstanceFromSearch(IDictionary<string, string> searchFields)
			{
				return new StandardTemplateItem(searchFields);
			}

			public ID InitializesTemplateId
			{
				get { return ID.Null; }
			}
		}

	}
}
