using System;
using Synthesis.Generation;
using System.Diagnostics.CodeAnalysis;

namespace Synthesis.Configuration
{
	/// <summary>
	/// Provides Generator Parameters using the Sitecore configuration API
	/// </summary>
	/// <example>
	/// <generatorParametersProvider type="Synthesis.Configuration.ConfigurationGeneratorParametersProvider, Synthesis">
	///		<ItemNamespace>Synthesis.Model.Concrete</ItemNamespace>
	///		<!-- etc - more elements with names matching properties on the parameters object -->
	///	</generatorParametersProvider>
	/// </example>
	public class ConfigurationGeneratorParametersProvider : GeneratorParameters, IGeneratorParametersProvider
	{
		public ConfigurationGeneratorParametersProvider()
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			InterfaceSuffix = "Item";
			SynthesisAssemblyPath = "~/bin/Synthesis.dll";
			SitecoreKernelAssemblyPath = "~/bin/Sitecore.Kernel.dll";
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
		}

		public GeneratorParameters CreateParameters(string configurationName)
		{
			this.ConfigurationName = configurationName;
			return this;
		}

		/// <summary>
		/// Translates a string type into a Type that the parameters expects to get
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification="It's a 'dropbox' property set via reflection from the XML config")]
		public string ItemBaseClassType
		{
			set
			{
				ItemBaseClass = Type.GetType(value);
			}
		}

		/// <summary>
		/// Translates a string type into a Type that the parameters expects to get
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1044:PropertiesShouldNotBeWriteOnly", Justification="It's a 'dropbox' property set via reflection from the XML config")]
		public string ItemBaseInterfaceType
		{
			set
			{
				ItemBaseInterface = Type.GetType(value);
			}
		}

		/// <summary>
		/// Proxies the config path resolution to the base class
		/// </summary>
		public override string InterfaceOutputPath
		{
			get
			{
				return base.InterfaceOutputPath;
			}
			set
			{
				base.InterfaceOutputPath = ConfigurationUtility.ResolveConfigurationPath(value);
			}
		}

		/// <summary>
		/// Proxies the config path resolution to the base class
		/// </summary>
		public override string ItemOutputPath
		{
			get
			{
				return base.ItemOutputPath;
			}
			set
			{
				base.ItemOutputPath = ConfigurationUtility.ResolveConfigurationPath(value);
			}
		}

		/// <summary>
		/// Proxies the config path resolution to the base class
		/// </summary>
		public override string SitecoreKernelAssemblyPath
		{
			get
			{
				return base.SitecoreKernelAssemblyPath;
			}
			set
			{
				base.SitecoreKernelAssemblyPath = ConfigurationUtility.ResolveConfigurationPath(value);
			}
		}

		/// <summary>
		/// Proxies the config path resolution to the base class
		/// </summary>
		public override string SynthesisAssemblyPath
		{
			get
			{
				return base.SynthesisAssemblyPath;
			}
			set
			{
				base.SynthesisAssemblyPath = ConfigurationUtility.ResolveConfigurationPath(value);
			}
		}
	}
}
