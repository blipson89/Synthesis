using System;
using System.Collections.Generic;
using System.IO;
using Sitecore.Diagnostics;
using Sitecore.StringExtensions;
using Synthesis.Generation.Model;

namespace Synthesis.Generation
{
	public class GeneratorParameters
	{
		private readonly List<TemplateGenerationMetadata> _friendMetadata = new List<TemplateGenerationMetadata>();

		public GeneratorParameters()
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			MaxBackupCopies = 5;
			UseTemplatePathForNamespace = true;
			ItemBaseClass = typeof(StandardTemplateItem);
			ItemBaseInterface = typeof(IStandardTemplateItem);
			// ReSharper restore DoNotCallOverridableMethodsInConstructor
		}

		/// <summary>
		/// Validates that all required parameters have been set and are valid values.
		/// Throws a <see cref="GeneratorParameterException"/> exception for invalid values.
		/// </summary>
		public void Validate()
		{
			if (string.IsNullOrEmpty(ItemNamespace))
				throw new GeneratorParameterException("ItemNamespace was not provided.");

			if (string.IsNullOrEmpty(InterfaceNamespace))
				throw new GeneratorParameterException("InterfaceNamespace was not provided.");

			if (InterfaceSuffix == null)
				throw new GeneratorParameterException("InterfaceSuffix was not provided.");

			if (string.IsNullOrEmpty(ItemOutputPath))
				throw new GeneratorParameterException("ItemOutputPath was not provided.");

			if (!ItemOutputPath.EndsWith(".cs"))
				throw new GeneratorParameterException("ItemOutputPath was not a C# file (.cs)");

			if (string.IsNullOrEmpty(InterfaceOutputPath))
				throw new GeneratorParameterException("InterfaceOutputPath was not provided.");

			if (!InterfaceOutputPath.EndsWith(".cs"))
				throw new GeneratorParameterException("InterfaceOutputPath was not a C# file (.cs)");

			if (string.IsNullOrEmpty(SitecoreKernelAssemblyPath) || !File.Exists(SitecoreKernelAssemblyPath))
				throw new GeneratorParameterException("SitecoreKernelAssemblyPath was not provided, or did not exist at the specified path.");

			if (string.IsNullOrEmpty(SynthesisAssemblyPath) || !File.Exists(SynthesisAssemblyPath))
				throw new GeneratorParameterException("SynthesisAssemblyPath was not provided, or did not exist at the specified path.");

			if (ItemBaseClass == null)
				throw new GeneratorParameterException("ItemBaseClass was not provided.");

			if (!typeof(StandardTemplateItem).IsAssignableFrom(ItemBaseClass))
				throw new GeneratorParameterException("ItemBaseClass " + ItemBaseClass.FullName + " did not derive from Synthesis.StandardTemplateItem.");

			if (ItemBaseInterface == null)
				throw new GeneratorParameterException("ItemBaseInterface was not provided.");

			if (!typeof(IStandardTemplateItem).IsAssignableFrom(ItemBaseInterface))
				throw new GeneratorParameterException("ItemBaseInterface " + ItemBaseInterface.FullName + " did not derive from Synthesis.IStandardTemplateItem.");

			if (!ItemBaseInterface.IsAssignableFrom(ItemBaseClass))
				throw new GeneratorParameterException(string.Format("ItemBaseClass {0} did not implement ItemBaseInterface {1}", ItemBaseClass.FullName, ItemBaseInterface.FullName));

			if(ConfigurationName.IsNullOrEmpty())
				throw new GeneratorParameterException("The configuration name was null or empty.");
		}

		/// <summary>
		/// Namespace for the generated items
		/// </summary>
		public virtual string ItemNamespace { get; set; }

		/// <summary>
		/// Namespace of generated item interfaces
		/// </summary>
		public virtual string InterfaceNamespace { get; set; }

		/// <summary>
		/// The suffix to append to interfaces after the template name. Defaults to "Item", e.g. "IFooItem" for a "Foo" template
		/// </summary>
		public virtual string InterfaceSuffix { get; set; }

		/// <summary>
		/// Path to write the output item source file to. Must have write access. Must end in ".cs"
		/// </summary>
		public virtual string ItemOutputPath { get; set; }

		/// <summary>
		/// Path to write the output interfaces source file to. Must have write access. Must end in ".cs"
		/// </summary>
		public virtual string InterfaceOutputPath { get; set; }

		/// <summary>
		/// Path to the Sitecore.Kernel.dll for referencing purposes.
		/// </summary>
		public virtual string SitecoreKernelAssemblyPath { get; set; }

		/// <summary>
		/// Path to the Synthesis.dll for referencing purposes.
		/// </summary>
		public virtual string SynthesisAssemblyPath { get; set; }

		/// <summary>
		/// Type of the base class for all items. Should derive from StandardTemplateItem if using something custom
		/// </summary>
		public virtual Type ItemBaseClass { get; set; }

		/// <summary>
		/// Type of the base interface for all items. Should derive from IStandardTemplateItem if using something custom.
		/// If using this you should almost certainly also be using a custom ItemBaseClass as well that must implement it.
		/// </summary>
		public virtual Type ItemBaseInterface { get; set; }

		/// <summary>
		/// When false: All model items and interfaces live in their root namespace regardless of Sitecore structure
		///	When true: All model items and interfaces live in namespaces based on their relative path in Sitecore
		///			The root of this relative path is controlled by the TemplatePathRoot setting.
		///						
		///	For example:
		///	With a template "/sitecore/Templates/MySite/Pages/Foo",
		///	and TemplatePathRoot "/sitecore/Templates/MySite",
		///	and InterfaceNamespace "MySite.Model",
		///	the template would be represented by MySite.Model.Pages.IFooItem
		/// </summary>
		public virtual bool UseTemplatePathForNamespace { get; set; }

		/// <summary>
		/// Sets the root Sitecore template path used to calculate output namespaces when the 
		///	UseTemplatePathForNamespace parameter is true.
		///	If UseTemplatePathForNamespace is false, this has no effect.
		/// </summary>
		public virtual string TemplatePathRoot { get; set; }

		/// <summary>
		/// Sets the number of backup copies the generator should keep of previous generated items
		/// </summary>
		public virtual uint MaxBackupCopies { get; set; }

		/// <summary>
		/// The name of the configuration being generated. Used to tag the generated classes.
		/// </summary>
		public virtual string ConfigurationName { get; set; }

		/// <summary>
		/// Adds friend metadata (e.g. a template built in a different configuration) which can be referenced by templates in this one as a base
		/// </summary>
		public virtual void AddFriendMetadata(TemplateGenerationMetadata metadata)
		{
			Assert.ArgumentNotNull(metadata, "metadata");
			_friendMetadata.Add(metadata);
		}

		internal IEnumerable<TemplateGenerationMetadata> GetFriendMetadata()
		{
			return _friendMetadata.AsReadOnly();
		}
	}
}
