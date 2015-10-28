using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Sitecore.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Synthesis.Utility;

namespace Synthesis.Configuration
{
	/// <summary>
	/// Provides a list of all types present in specified assembly names. This provider is appropriate when Synthesis classes and presenters are in a known set of places.
	/// It is also much faster than AppDomainTypeListProvider.
	/// </summary>
	public class ConfigurationTypeListProvider : ITypeListProvider
	{
		private static volatile List<Type> _types;
		private readonly List<Assembly> _assemblies = new List<Assembly>();

		private static readonly object SyncRoot = new object();

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to ignore all load errors on assembly types")]
		public IEnumerable<Type> CreateTypeList()
		{
			if (_types == null)
			{
				lock (SyncRoot)
				{
					if (_types == null)
					{
						var timer = new Stopwatch();
						timer.Start();

						IEnumerable<Assembly> assemblies;

						if (_assemblies.Count > 0)
							assemblies = _assemblies;
						else assemblies = AppDomain.CurrentDomain.GetAssemblies();

						_types = assemblies.SelectMany(delegate(Assembly x)
							{
								try { return x.GetTypes(); }
								catch (ReflectionTypeLoadException rex) { return rex.Types.Where(y => y != null).ToArray(); } // http://haacked.com/archive/2012/07/23/get-all-types-in-an-assembly.aspx
								catch { return new Type[] { }; }
							}).ToList();

						timer.Stop();
						Log.Info(string.Format("Synthesis: Loaded types from {0} in {1}ms", (_assemblies.Count == 0) ? "AppDomain" : "assemblies", timer.ElapsedMilliseconds), this);
					}
				}
			}

			return _types;
		}

		public void AddAssembly(string name)
		{
			if (name.Contains("*"))
			{
				var assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (var assembly in assemblies)
				{
					if (WildcardUtility.IsWildcardMatch(assembly.FullName, name)) AddAssembly(assembly.FullName);
				}

				return;
			}

			Assembly a = Assembly.Load(name);
			if (a == null) throw new ArgumentException("The assembly name was not valid");

			_types = null;
			_assemblies.Add(a);
		}
	}
}
