using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.StringExtensions;
using Synthesis.Configuration;
using Synthesis.Utility;

namespace Synthesis.Initializers
{
	public class StandardInitializerProvider : IInitializerProvider
	{
		private readonly ITypeListProvider _typeListProvider;
		private ReadOnlyDictionary<ID, ITemplateInitializer> _initializerCache;
		private static readonly object _lock = new object();

		public StandardInitializerProvider(ITypeListProvider typeListProvider)
		{
			_typeListProvider = typeListProvider;
		}

		protected virtual void EnsureInitialized()
		{
			if (_initializerCache != null) return;

			lock (_lock)
			{
				if (_initializerCache != null) return;

				var timer = new Stopwatch();
				timer.Start();

				var initializers = new Dictionary<ID, ITemplateInitializer>();

				var initializerTypes = _typeListProvider.CreateTypeList().ImplementingInterface(typeof (ITemplateInitializer));

				foreach (var initializer in initializerTypes)
				{
					var instance = (ITemplateInitializer) Activator.CreateInstance(initializer);

					if (initializers.ContainsKey(instance.InitializesTemplateId))
					{
						throw new InvalidOperationException("Synthesis: Multiple initializers were found for template {0} ({1}, {2}).".FormatWith(instance.InitializesTemplateId, initializers[instance.InitializesTemplateId].GetType().FullName, instance.GetType().FullName));
					}

					// ignore test and standard template initializers
					if (instance.InitializesTemplateId == ID.Null) continue;

					initializers.Add(instance.InitializesTemplateId, instance);
				}

				_initializerCache = new ReadOnlyDictionary<ID, ITemplateInitializer>(initializers);

				timer.Stop();
				Log.Info("Synthesis: Initialized template initializer cache (" + _initializerCache.Count + " templates) in " + timer.ElapsedMilliseconds + " ms", _initializerCache);
			}
		}

		public ITemplateInitializer GetInitializer(ID templateId)
		{
			EnsureInitialized();

			ITemplateInitializer initializer;
			if (_initializerCache.TryGetValue(templateId, out initializer)) return initializer;

			return null;
		}
	}
}
