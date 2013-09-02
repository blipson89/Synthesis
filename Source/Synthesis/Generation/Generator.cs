using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Synthesis.FieldTypes;
using Synthesis.Synchronization;
using Synthesis.Templates;
using Synthesis.Utility;
using System.CodeDom.Compiler;
using System.Reflection;
using Sitecore.ContentSearch;
using Synthesis.ContentSearch;

namespace Synthesis.Generation
{
	public class Generator
	{
		private GeneratorParameters Parameters { get; set; }
		private TemplateGenerationInfoCollection Templates { get; set; }
		private HashSet<string> _baseClassFields;
		private readonly IFieldMappingProvider _fieldMappingProvider;
		private readonly ITemplateInputProvider _templateInputProvider;
		private readonly ITemplateSignatureProvider _templateSignatureProvider;
		private readonly ISynthesisIndexFieldNameTranslator _indexFieldNameTranslator;

		const string StandardTemplate = "STANDARD TEMPLATE";

		public Generator(IGeneratorParametersProvider parameterProvider, ITemplateInputProvider templateProvider, ITemplateSignatureProvider templateSignatureProvider, IFieldMappingProvider fieldMappingProvider, ISynthesisIndexFieldNameTranslator indexFieldNameTranslator) 
			:this(parameterProvider.CreateParameters(), templateProvider, templateSignatureProvider, fieldMappingProvider, indexFieldNameTranslator)
		{
		}

		public Generator(GeneratorParameters parameters, ITemplateInputProvider templateProvider, ITemplateSignatureProvider templateSignatureProvider, IFieldMappingProvider fieldMappingProvider, ISynthesisIndexFieldNameTranslator indexFieldNameTranslator)
		{
			parameters.Validate();
			Parameters = parameters;

			_templateInputProvider = templateProvider;
			_templateSignatureProvider = templateSignatureProvider;
			_fieldMappingProvider = fieldMappingProvider;
			_indexFieldNameTranslator = indexFieldNameTranslator;

			// load the templates we'll be generating into a state storage collection
			var templates = templateProvider.CreateTemplateList();
			Templates = new TemplateGenerationInfoCollection(parameters.UseTemplatePathForNamespace, parameters.TemplatePathRoot);
			foreach (var template in templates) Templates.Add(template);
		}

		/// <summary>
		/// Generates source code files and writes them to disk
		/// </summary>
		public void GenerateToDisk()
		{
			var timer = new Stopwatch();
			timer.Start();

			// for source generation we want to split the generated code into separate units (files)
			CodeCompileUnit concreteUnit = CreateCodeCompileUnit();
			CodeCompileUnit interfaceUnit = CreateCodeCompileUnit();

			GenerateTemplateCode(concreteUnit, interfaceUnit);

			SortCodeCompileUnit(concreteUnit);
			SortCodeCompileUnit(interfaceUnit);

			WriteFileWithBackups(Parameters.ItemOutputPath, concreteUnit);
			WriteFileWithBackups(Parameters.InterfaceOutputPath, interfaceUnit);

			timer.Stop();
			Log.Info(string.Format("Synthesis: Generated models for {0} templates in {1} ms", Templates.Count, timer.ElapsedMilliseconds), this);
		}

		/// <summary>
		/// Generates code representing a set of Sitecore templates
		/// </summary>
		private void GenerateTemplateCode(CodeCompileUnit concreteUnit, CodeCompileUnit interfaceUnit)
		{
			CodeNamespace interfaceNamespace = interfaceUnit.CreateNamespace(Parameters.InterfaceNamespace);

			var templatesToGenerate = new List<TemplateGenerationInfo>(Templates); // as interface generation can modify the Templates collection, we need to make an immutable copy of the collection items before we iterate over it

			foreach (var template in templatesToGenerate)
			{
				// setup concrete model object
				CodeNamespace templateNamespace = GetTemplateNamespace(concreteUnit, template.Template.ID, Parameters.ItemNamespace);
				CodeTypeDeclaration concrete = templateNamespace.CreateType(MemberAttributes.Public, template.TypeName);
				concrete.IsClass = true;
				concrete.IsPartial = true;
				concrete.BaseTypes.Add(new CodeTypeReference(Parameters.ItemBaseClass, CodeTypeReferenceOptions.GlobalReference));

				// add the constructor (validates item template)
				concrete.Members.AddRange(CreateItemConstructors());

				AddGeneratedCodeAttributeToType(concrete);
				AddTemplateIdPropertiesToEntity(concrete, template.Template); // adds static and dynamic template ID property
				AddCommentsToItem(concrete, template.Template); // adds XML comments based on Sitecore Help fields

				HashSet<string> fieldKeys = GetBaseFieldSet();
				fieldKeys.Add(concrete.Name.AsIdentifier()); // member names cannot be the same as their enclosing type so we add the type name to the fields collection
				foreach (var baseTemplate in template.AllNonstandardBaseTemplates) // similarly names can't be the same as any of their base templates' names (this would cause an incompletely implemented interface)
					fieldKeys.Add(baseTemplate.Template.Name.AsIdentifier());		// NOTE: you could break this if you have a base template called Foo and a field called Foo that IS NOT on the Foo template (but why would you have that?)
				
				// generate item properties
				foreach (var field in template.Template.Fields)
				{
					if (_templateInputProvider.IsFieldIncluded(field)) // query the template input provider and make sure we generate
					{
						string propertyName = field.Name.AsNovelIdentifier(fieldKeys);
						bool propertyAdded = CreateItemProperty(propertyName, field, concrete.Members);

						if (propertyAdded)
						{
							// record usage of the property name
							fieldKeys.Add(propertyName);
						}
					}
				}

				// generates interfaces to represent the Sitecore template inheritance hierarchy
				string baseInterface = GenerateInheritedInterfaces(template.Template, interfaceUnit, interfaceNamespace);
				if (!string.IsNullOrEmpty(baseInterface))
					concrete.BaseTypes.Add(new CodeTypeReference(baseInterface, CodeTypeReferenceOptions.GlobalReference)); // implement the base type interface

				// create initializer class
				CreateInitializer(templateNamespace, concrete, template.Template.ID);
			}
		}

		/// <summary>
		/// Generates an Initializer class (a proxy that allows us to quickly construct instances of a strongly typed item wrapper without resorting to reflection)
		/// </summary>
		private void CreateInitializer(CodeNamespace templateNamespace, CodeTypeDeclaration concrete, ID templateId)
		{
			CodeTypeDeclaration initializer = templateNamespace.CreateType(MemberAttributes.Public, concrete.Name + "Initializer");
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
			initializer.Attributes = MemberAttributes.Public | MemberAttributes.Final;
// ReSharper restore BitwiseOperatorOnEnumWithoutFlags
			initializer.BaseTypes.Add(typeof(ITemplateInitializer));
			AddGeneratedCodeAttributeToType(initializer);

			var templateIdProperty = new CodeMemberProperty
				{
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
					Attributes = MemberAttributes.Public | MemberAttributes.Final,
// ReSharper restore BitwiseOperatorOnEnumWithoutFlags
					Type = new CodeTypeReference(typeof (ID)),
					Name = "InitializesTemplateId"
				};

			// all this for return new ID("xxxxx"); lol
			templateIdProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(typeof(ID), new CodeSnippetExpression("\"" + templateId + "\""))));

			initializer.Members.Add(templateIdProperty);

			var initializerItemMethod = new CodeMemberMethod
				{
					Name = "CreateInstance",
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
					Attributes = MemberAttributes.Public | MemberAttributes.Final,
// ReSharper restore BitwiseOperatorOnEnumWithoutFlags
					ReturnType = new CodeTypeReference(typeof (IStandardTemplateItem))
				};

			initializerItemMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Item), "innerItem"));

			// all this for return new ConcreteType(innerItem); lol
			initializerItemMethod.Statements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(new CodeTypeReference(concrete.Name), new CodeVariableReferenceExpression("innerItem"))));

			initializer.Members.Add(initializerItemMethod);

			var initializerIndexMethod = new CodeMemberMethod
			{
				Name = "CreateInstanceFromSearch",
				// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
				Attributes = MemberAttributes.Public | MemberAttributes.Final,
				// ReSharper restore BitwiseOperatorOnEnumWithoutFlags
				ReturnType = new CodeTypeReference(typeof(IStandardTemplateItem))
			};

			initializerIndexMethod.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IDictionary<string, string>), CodeTypeReferenceOptions.GlobalReference), "searchFields"));

			// all this for return new ConcreteType(searchFields); lol
			initializerIndexMethod.Statements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(new CodeTypeReference(concrete.Name), new CodeVariableReferenceExpression("searchFields"))));

			initializer.Members.Add(initializerIndexMethod);
		}

		/// <summary>
		/// Generates an interface for each template that the current template derives from, recursively.
		/// </summary>
		private string GenerateInheritedInterfaces(TemplateItem template, CodeCompileUnit codeUnit, CodeNamespace interfaceNamespace)
		{
			// some interfaces may come from outside the valid-for-generation template paths. In that case, we want to make them referenceable for unique naming
			if (!Templates.Contains(template.ID))
			{
				Templates.Add(template);
			}

			TemplateGenerationInfo info = Templates[template.ID];
			// note: the info.InterfaceFullName causes generation to be skipped if we already generated this interface (as this is a recursive method and several templates can derive from the same base)
			// the check against standard template breaks the recursion chain
			if (string.IsNullOrEmpty(info.InterfaceFullName) && template.Name.ToUpperInvariant() != StandardTemplate)
			{
				string interfaceTypeName = string.Concat("I", info.TypeName, Parameters.InterfaceSuffix);
				CodeTypeDeclaration interfaceType = GetTemplateNamespace(codeUnit, template.ID, interfaceNamespace.Name).CreateType(MemberAttributes.Public, interfaceTypeName);
				interfaceType.IsInterface = true;
				interfaceType.IsPartial = true;

				// store the interface's full type name in the Templates collection for later use
				info.InterfaceFullName = string.Concat(info.GetNamespace(Parameters.InterfaceNamespace), ".", interfaceTypeName);

				AddGeneratedCodeAttributeToType(interfaceType);
				AddVersionAttributeToInterface(interfaceType, template); // adds version data to the interface (for synchronization purposes)
				AddCommentsToItem(interfaceType, template); // add XML comments based on Sitecore Help content

				var fieldKeys = GetBaseFieldSet();
				fieldKeys.Add(interfaceType.Name.AsIdentifier()); // member names cannot be the same as their enclosing type
				fieldKeys.Add(info.TypeName); // prevent any fields from being generated that would conflict with a concrete item name as well as the interface name

				// generate interface properties
				foreach (var field in template.OwnFields)
				{
					if (_templateInputProvider.IsFieldIncluded(field))
					{
						string propertyName = field.Name.AsNovelIdentifier(fieldKeys);
						CodeMemberProperty property = CreateInterfaceProperty(propertyName, field);

						if (property != null)
						{
							interfaceType.Members.Add(property);
							fieldKeys.Add(propertyName);
						}
					}
				}

				// add base interface inheritance
				foreach (var baseTemplate in template.BaseTemplates)
				{
					if (baseTemplate.Name.ToUpperInvariant() != StandardTemplate)
					{
						// recursively generate base templates' interfaces as needed
						string baseInterface = GenerateInheritedInterfaces(baseTemplate, codeUnit, interfaceNamespace);
						if (!string.IsNullOrEmpty(baseInterface))
							interfaceType.BaseTypes.Add(new CodeTypeReference(baseInterface, CodeTypeReferenceOptions.GlobalReference)); // assign interface implementation
					}
				}
				
				// if this interface has no bases make sure we're deriving from the standard template
				if (interfaceType.BaseTypes.Count == 0 || (interfaceType.BaseTypes.Count == 1 && interfaceType.BaseTypes[0].BaseType.Contains(Parameters.ItemBaseInterface.FullName))) // any base types indicate it's inheriting from another sitecore interface - and we don't want to dupe the base class fields twice
				{
					interfaceType.BaseTypes.Add(new CodeTypeReference(Parameters.ItemBaseInterface));
				}

				return info.InterfaceFullName;
			}

			if (!string.IsNullOrEmpty(info.InterfaceFullName))
			{
				return info.InterfaceFullName;
			}

			return null;
		}

		/// <summary>
		/// Adds the versioning control attribute to the interface
		/// </summary>
		/// <example>[RepresentsSitecoreTemplate("{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}", "Dn8cOiiO0ckeD/NPjd9Q8nJuPSk=")]</example>
		private void AddVersionAttributeToInterface(CodeTypeDeclaration interfaceType, TemplateItem template)
		{
			var attribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(RepresentsSitecoreTemplateAttribute)));
			
			attribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(template.ID.ToString())));
			attribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_templateSignatureProvider.GenerateTemplateSignature(template))));

			interfaceType.CustomAttributes.Add(attribute);
		}

		System.Version _synthesisVersion;

		/// <summary>
		/// Adds the GeneratedCodeAttribute to a generated class/interface
		/// </summary>
		/// <example>[System.CodeDom.Compiler.GeneratedCode("Synthesis", "4.2")]</example>
		private void AddGeneratedCodeAttributeToType(CodeTypeDeclaration type)
		{
			var attribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(GeneratedCodeAttribute), CodeTypeReferenceOptions.GlobalReference));

			attribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("Synthesis")));

			if (_synthesisVersion == null)
				_synthesisVersion = Assembly.GetExecutingAssembly().GetName().Version;

			attribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_synthesisVersion.ToString())));

			type.CustomAttributes.Add(attribute);
		}

		#region Code Snippet Generation
		private static CodeTypeMember[] CreateItemConstructors()
		{
			var itemConstructor = new CodeConstructor();
			itemConstructor.Attributes = MemberAttributes.Public;

			itemConstructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(Item), "innerItem"));
			itemConstructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("innerItem"));

			var indexConstructor = new CodeConstructor();
			indexConstructor.Attributes = MemberAttributes.Public;

			indexConstructor.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(IDictionary<string, string>), CodeTypeReferenceOptions.GlobalReference), "searchFields"));
			indexConstructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("searchFields"));

			return new[] {itemConstructor, indexConstructor};
		}

		private bool CreateItemProperty(string propertyName, TemplateFieldItem sitecoreField, CodeTypeMemberCollection members)
		{
			var type = _fieldMappingProvider.GetFieldType(sitecoreField);

			if(type == null)
			{
				Log.Warn("Synthesis: Field type resolution for " + sitecoreField.InnerItem.Parent.Parent.Name + "::" + sitecoreField.Name + " failed; no mapping found for field type " + sitecoreField.Type, this);
				return false;
			}

			var backingFieldName = "_" + propertyName[0].ToString(CultureInfo.InvariantCulture).ToLower() + propertyName.Substring(1);
			var backingField = new CodeMemberField(new CodeTypeReference(type.InternalFieldType), backingFieldName);

			backingField.Attributes = MemberAttributes.Private;

			var property = new CodeMemberProperty
				{
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
					Attributes = MemberAttributes.Public | MemberAttributes.Final,
// ReSharper restore BitwiseOperatorOnEnumWithoutFlags
					Type = new CodeTypeReference(type.PublicFieldType),
					Name = propertyName,
					HasGet = true
				};

			// add [IndexField] attribute
			property.CustomAttributes.Add(GetIndexFieldAttribute(sitecoreField.Name));

			// if(backingField == null)
			//	backingField = new SynthesisFieldType(new Lazy<Field>(() => InnerItem.Fields["xxx"], GetSearchFieldValue("index-field-name"));

			var initializerLambda = new CodeSnippetExpression(string.Format("new global::Synthesis.FieldTypes.LazyField(() => InnerItem.Fields[\"{0}\"], \"{1}\", \"{2}\")", sitecoreField.ID, sitecoreField.Template.InnerItem.Paths.FullPath, sitecoreField.Name));
			var initializerSearchReference = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(),
																			"GetSearchFieldValue",
																			new CodePrimitiveExpression(_indexFieldNameTranslator.GetIndexFieldName(sitecoreField.Name)));

			var backingFieldNullCheck = new CodeConditionStatement();
			backingFieldNullCheck.Condition = new CodeSnippetExpression(string.Format("{0} == null", backingFieldName));
			backingFieldNullCheck.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(backingFieldName), new CodeObjectCreateExpression(type.InternalFieldType, initializerLambda, initializerSearchReference)));
			property.GetStatements.Add(backingFieldNullCheck);

			// return backingField;
			property.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(backingFieldName)));
			
			AddCommentsToFieldProperty(property, sitecoreField);

			members.Add(backingField);
			members.Add(property);

			return true;
		}

		private CodeMemberProperty CreateInterfaceProperty(string propertyName, TemplateFieldItem sitecoreField)
		{
			var type = _fieldMappingProvider.GetFieldType(sitecoreField);

			if (type == null)
			{
				Log.Warn("Synthesis: Field type resolution (interface) for " + sitecoreField.InnerItem.Parent.Parent.Name + "::" + sitecoreField.Name + " failed; no mapping found for field type " + sitecoreField.Type, this);
				return null;
			}

			var property = new CodeMemberProperty
				{
					Type = new CodeTypeReference(type.PublicFieldType), 
					Name = propertyName, 
					HasGet = true
				};

			// add [IndexField] attribute
			property.CustomAttributes.Add(GetIndexFieldAttribute(sitecoreField.Name));

			AddCommentsToFieldProperty(property, sitecoreField);

			return property;
		}

		private CodeAttributeDeclaration GetIndexFieldAttribute(string fieldName)
		{
			// this is effectively [IndexField("index_field_name")]
			var indexFieldName = _indexFieldNameTranslator.GetIndexFieldName(fieldName);
			return new CodeAttributeDeclaration(new CodeTypeReference(typeof(IndexFieldAttribute)),
															new CodeAttributeArgument(new CodePrimitiveExpression(indexFieldName)));
		}

		/// <summary>
		/// Adds static and instance properties to an entity type to allow access to its TemplateID programmatically
		/// </summary>
		private static void AddTemplateIdPropertiesToEntity(CodeTypeDeclaration entity, TemplateItem template)
		{
			var templateNameStaticProperty = new CodeMemberProperty
			{
				Type = new CodeTypeReference(typeof(string)),
				Name = "TemplateName",
				HasGet = true,
				HasSet = false,
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
				Attributes = MemberAttributes.Public | MemberAttributes.Static
// ReSharper restore BitwiseOperatorOnEnumWithoutFlags
			};

			templateNameStaticProperty.Comments.Add(new CodeCommentStatement("<summary>The name of the Sitecore Template that this class represents</summary>", true));

			templateNameStaticProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(template.Name)));

			entity.Members.Add(templateNameStaticProperty);

			var templateIdStaticProperty = new CodeMemberProperty
			{
				Type = new CodeTypeReference(typeof(ID)),
				Name = "ItemTemplateId",
				HasGet = true,
				HasSet = false,
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
				Attributes = MemberAttributes.Public | MemberAttributes.Static
// ReSharper restore BitwiseOperatorOnEnumWithoutFlags
			};

			templateIdStaticProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("new Sitecore.Data.ID(\"" + template.ID + "\")")));

			templateIdStaticProperty.Comments.Add(new CodeCommentStatement("<summary>The ID of the Sitecore Template that this class represents</summary>", true));

			entity.Members.Add(templateIdStaticProperty);

			var templateIdInstanceProperty = new CodeMemberProperty
			{
				Type = new CodeTypeReference(typeof(ID)),
				Name = "TemplateId",
				HasGet = true,
				HasSet = false,
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
				Attributes = MemberAttributes.Public | MemberAttributes.Override
// ReSharper restore BitwiseOperatorOnEnumWithoutFlags
			};

			templateIdInstanceProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("ItemTemplateId")));

			templateIdInstanceProperty.Comments.Add(new CodeCommentStatement("<summary>The ID of the Sitecore Template that this class represents</summary>", true));

			entity.Members.Add(templateIdInstanceProperty);
		}

		private static void AddCommentsToItem(CodeTypeDeclaration entity, TemplateItem template)
		{
			if (!string.IsNullOrEmpty(template.InnerItem.Help.Text))
				entity.Comments.Add(new CodeCommentStatement("<summary>" + template.InnerItem.Help.Text + "</summary>", true));
			else
				entity.Comments.Add(new CodeCommentStatement(string.Format("<summary>Represents the {0} template</summary>", template.InnerItem.Paths.FullPath), true));
		}

		private static void AddCommentsToFieldProperty(CodeMemberProperty property, TemplateFieldItem field)
		{
			if (!string.IsNullOrEmpty(field.Description))
				property.Comments.Add(new CodeCommentStatement("<summary>" + field.Description + "</summary>", true));
			else
				property.Comments.Add(new CodeCommentStatement(string.Format("<summary>Represents the {0} field</summary>", field.DisplayName), true));
		}
		#endregion

		/// <summary>
		/// Gets the writeable property names defined in the item base class (normally StandardTemplateItem) so we generate unique field names for any fields that have the same names as these
		/// </summary>
		private HashSet<string> GetBaseFieldSet()
		{
			if (_baseClassFields == null)
			{
				_baseClassFields = new HashSet<string>();

				// we ignore adding the properties that already exist in the entity base class when making entities
				var baseProperties = Parameters.ItemBaseClass.GetProperties();
				foreach (var prop in baseProperties)
					_baseClassFields.Add(prop.Name);
			}

			return new HashSet<string>(_baseClassFields);
		}

		/// <summary>
		/// Writes a file to disk and creates backup copies if a file with the same name already exists
		/// </summary>
		/// <remarks>Backups are numbered i.e. .1, .2, .3, .4, etc and rotate up as new copies are made (i.e. if .1 exists, it will be renamed .2 and the current one renamed to .1, etc)</remarks>
		private void WriteFileWithBackups(string path, CodeCompileUnit code)
		{
			if (File.Exists(path)) // existing version present; make backups as necessary
			{
				uint maxBackups = Parameters.MaxBackupCopies;

				uint i = maxBackups;
				do // find and move existing backups if needed
				{
					string instanceFileName = path + "." + i.ToString(CultureInfo.InvariantCulture);
					bool instanceExists = File.Exists(instanceFileName);

					if (i == maxBackups && instanceExists) File.Delete(instanceFileName); // truncate a backup that's too old
					if (i > 0 && i != maxBackups && instanceExists) File.Move(instanceFileName, string.Format("{0}.{1}", path, (i + 1).ToString(CultureInfo.InvariantCulture))); // move an existing backup up a number in the backups

					i--;
				} while (i > 0);

				// move the pre-existing file into the file1.ext position
				File.Move(path, path + ".1");
			}

			// write new file
			File.WriteAllText(path, code.CompileToCSharpSourceCode());
		}

		/// <summary>
		/// Gets the namespace that a template should live in. Depending on the UseTemplatePathForNamespace setting, this may change on a per-template basis.
		/// </summary>
		/// <param name="unit">Code unit the namespace belongs in</param>
		/// <param name="templateId">ID of the sitecore template</param>
		/// <param name="rootNamespace">The default namespace the template belongs in</param>
		private CodeNamespace GetTemplateNamespace(CodeCompileUnit unit, ID templateId, string rootNamespace)
		{
			if (!Templates.Contains(templateId)) throw new ArgumentException("Unknown template, cannot resolve namespace.");

			return GetNamespaceWithCreate(unit, Templates[templateId].GetNamespace(rootNamespace));
		}

		/// <summary>
		/// Gets a namespace reference. If it already exists it will be returned; else it will be created first.
		/// </summary>
		private static CodeNamespace GetNamespaceWithCreate(CodeCompileUnit unit, string namespaceToGet)
		{
			CodeNamespace ns = unit.Namespaces.Cast<CodeNamespace>().SingleOrDefault(x => x.Name == namespaceToGet);

			if (ns == null)
			{
				ns = new CodeNamespace(namespaceToGet);
				unit.Namespaces.Add(ns);
			}

			return ns;
		}

		private CodeCompileUnit CreateCodeCompileUnit()
		{
			var codeUnit = new CodeCompileUnit();

			// required library references to compile
			codeUnit.ReferencedAssemblies.Add(Parameters.SitecoreKernelAssemblyPath);
			codeUnit.ReferencedAssemblies.Add(Parameters.SynthesisAssemblyPath);

			return codeUnit;
		}

		private void SortCodeCompileUnit(CodeCompileUnit unit)
		{
			var namespaces = new CodeNamespace[unit.Namespaces.Count];
			unit.Namespaces.CopyTo(namespaces, 0);
			Array.Sort(namespaces, (ns, codeNamespace) => ns.Name.CompareTo(codeNamespace.Name));
			unit.Namespaces.Clear();
			unit.Namespaces.AddRange(namespaces);

			foreach (CodeNamespace ns in unit.Namespaces)
			{
				var types = new CodeTypeDeclaration[ns.Types.Count];
				ns.Types.CopyTo(types, 0);
				Array.Sort(types, (declaration, typeDeclaration) => declaration.Name.CompareTo(typeDeclaration.Name));
				ns.Types.Clear();
				ns.Types.AddRange(types);
			}
		}
	}
}
