using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Synthesis.Generation.Model;
using Synthesis.Initializers;
using Synthesis.Synchronization;
using Synthesis.Templates;
using Synthesis.Utility;

namespace Synthesis.Generation.CodeDom
{
	public class CodeDomGenerator : ITemplateCodeGenerator
	{
		private TemplateGenerationMetadata _metadata;
		private GeneratorParameters _parameters;
		private readonly ITemplateSignatureProvider _signatureProvider;

		public CodeDomGenerator(ITemplateSignatureProvider signatureProvider)
		{
			_signatureProvider = signatureProvider;
		}

		/// <summary>
		/// Generates source code files and writes them to disk
		/// </summary>
		public void Generate(TemplateGenerationMetadata metadata)
		{
			// this is a hack hack hack. Use parameters, lazybones.
			_metadata = metadata;
			_parameters = metadata.Parameters;

			// for source generation we want to split the generated code into separate units (files)
			CodeCompileUnit interfaceUnit = CreateCodeCompileUnit();
			CodeCompileUnit concreteUnit = CreateCodeCompileUnit();

			GenerateInterfaceCode(interfaceUnit);
			GenerateTemplateCode(concreteUnit);

			SortCodeCompileUnit(concreteUnit);
			SortCodeCompileUnit(interfaceUnit);

			WriteFileWithBackups(_parameters.ItemOutputPath, concreteUnit);
			WriteFileWithBackups(_parameters.InterfaceOutputPath, interfaceUnit);
		}

		/// <summary>
		/// Generates code representing a set of Sitecore templates
		/// </summary>
		private void GenerateTemplateCode(CodeCompileUnit concreteUnit)
		{
			foreach (var template in _metadata.Templates)
			{
				// setup concrete model object
				CodeNamespace templateNamespace = GetNamespaceWithCreate(concreteUnit, template.Namespace);
				CodeTypeDeclaration concrete = templateNamespace.CreateType(MemberAttributes.Public, template.TypeName.AsIdentifier());
				concrete.IsClass = true;
				concrete.IsPartial = true;
				concrete.BaseTypes.Add(new CodeTypeReference(_parameters.ItemBaseClass, CodeTypeReferenceOptions.GlobalReference));

				// add the constructor (validates item template)
				concrete.Members.AddRange(CreateItemConstructors());

				AddGeneratedCodeAttributeToType(concrete);
				AddTemplateIdPropertiesToEntity(concrete, template.Template); // adds static and dynamic template ID property
				AddCommentsToItem(concrete, template.Template); // adds XML comments based on Sitecore Help fields


				// generate item properties
				foreach (var field in template.FieldsToGenerate)
				{
					CreateItemProperty(field, concrete.Members);
				}

				// generates interfaces to represent the Sitecore template inheritance hierarchy
				foreach (var baseType in template.InterfacesImplemented)
				{
					concrete.BaseTypes.Add(new CodeTypeReference(baseType.TypeFullName, CodeTypeReferenceOptions.GlobalReference)); // implement the base type interface
				}

				// create initializer class
				CreateInitializer(templateNamespace, concrete, template.Template.TemplateId);
			}
		}

		/// <summary>
		/// Generates an interface for each template that the current template derives from, recursively.
		/// </summary>
		private void GenerateInterfaceCode(CodeCompileUnit interfaceUnit)
		{
			foreach (var template in _metadata.Interfaces)
			{
				CodeNamespace interfaceNamespace = GetNamespaceWithCreate(interfaceUnit, template.Namespace);
				CodeTypeDeclaration interfaceType = interfaceNamespace.CreateType(MemberAttributes.Public, template.TypeName);
				interfaceType.IsInterface = true;
				interfaceType.IsPartial = true;

				AddGeneratedCodeAttributeToType(interfaceType);
				AddVersionAttributeToInterface(interfaceType, template.Template); // adds version data to the interface (for synchronization purposes)
				AddCommentsToItem(interfaceType, template.Template); // add XML comments based on Sitecore Help content

				// generate interface properties
				foreach (var field in template.FieldsToGenerate)
				{	
					CreateInterfaceProperty(field, interfaceType.Members);
				}

				// add base interface inheritance
				foreach (var baseTemplate in template.InterfacesImplemented)
				{
					interfaceType.BaseTypes.Add(new CodeTypeReference(baseTemplate.TypeFullName, CodeTypeReferenceOptions.GlobalReference)); // assign interface implementation
				}

				// if this interface has no bases make sure we're deriving from the standard template
				if (interfaceType.BaseTypes.Count == 0 || (interfaceType.BaseTypes.Count == 1 && interfaceType.BaseTypes[0].BaseType.Contains(_parameters.ItemBaseInterface.FullName))) // any base types indicate it's inheriting from another sitecore interface - and we don't want to dupe the base class fields twice
				{
					interfaceType.BaseTypes.Add(new CodeTypeReference(_parameters.ItemBaseInterface));
				}
			}
		}

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

			return new CodeTypeMember[] { itemConstructor, indexConstructor };
		}

		/// <summary>
		/// Adds static and instance properties to an entity type to allow access to its TemplateID programmatically
		/// </summary>
		private static void AddTemplateIdPropertiesToEntity(CodeTypeDeclaration entity, ITemplateInfo template)
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

			templateIdStaticProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("new Sitecore.Data.ID(\"" + template.TemplateId + "\")")));

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

		private static void AddCommentsToItem(CodeTypeDeclaration entity, ITemplateInfo template)
		{
			if (!string.IsNullOrEmpty(template.HelpText))
				entity.Comments.Add(new CodeCommentStatement("<summary>" + template.HelpText + "</summary>", true));
			else
				entity.Comments.Add(new CodeCommentStatement(string.Format("<summary>Represents the {0} template</summary>", template.FullPath), true));
		}

		private static void AddCommentsToFieldProperty(CodeMemberProperty property, ITemplateFieldInfo field)
		{
			if (!string.IsNullOrEmpty(field.HelpText))
				property.Comments.Add(new CodeCommentStatement("<summary>" + field.HelpText + "</summary>", true));
			else
				property.Comments.Add(new CodeCommentStatement(string.Format("<summary>Represents the {0} field</summary>", field.DisplayName), true));
		}

		private void CreateItemProperty(FieldPropertyInfo propertyInfo, CodeTypeMemberCollection members)
		{
			Assert.ArgumentNotNull(propertyInfo, "propertyInfo");

			var backingFieldName = "_" + propertyInfo.FieldPropertyName[0].ToString(CultureInfo.InvariantCulture).ToLower() + propertyInfo.FieldPropertyName.Substring(1);
			var backingField = new CodeMemberField(new CodeTypeReference(propertyInfo.FieldType.InternalFieldType), backingFieldName);

			backingField.Attributes = MemberAttributes.Private;

			var property = new CodeMemberProperty
			{
				// ReSharper disable BitwiseOperatorOnEnumWithoutFlags
				Attributes = MemberAttributes.Public | MemberAttributes.Final,
				// ReSharper restore BitwiseOperatorOnEnumWithoutFlags
				Type = new CodeTypeReference(propertyInfo.FieldType.PublicFieldType),
				Name = propertyInfo.FieldPropertyName,
				HasGet = true
			};

			// add [IndexField] attribute
			property.CustomAttributes.Add(GetIndexFieldAttribute(propertyInfo.SearchFieldName));

			// if(backingField == null)
			//	backingField = new SynthesisFieldType(new Lazy<Field>(() => InnerItem.Fields["xxx"], GetSearchFieldValue("index-field-name"));

			var initializerLambda = new CodeSnippetExpression(string.Format("new global::Synthesis.FieldTypes.LazyField(() => InnerItem.Fields[\"{0}\"], \"{1}\", \"{2}\")", propertyInfo.Field.Id, propertyInfo.Field.Template.FullPath, propertyInfo.Field.Name));
			var initializerSearchReference = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(),
																			"GetSearchFieldValue",
																			new CodePrimitiveExpression(propertyInfo.SearchFieldName));

			var backingFieldNullCheck = new CodeConditionStatement();
			backingFieldNullCheck.Condition = new CodeSnippetExpression(string.Format("{0} == null", backingFieldName));
			backingFieldNullCheck.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression(backingFieldName), new CodeObjectCreateExpression(propertyInfo.FieldType.InternalFieldType, initializerLambda, initializerSearchReference)));
			property.GetStatements.Add(backingFieldNullCheck);

			// return backingField;
			property.GetStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(backingFieldName)));

			AddCommentsToFieldProperty(property, propertyInfo.Field);

			members.Add(backingField);
			members.Add(property);
		}

		private void CreateInterfaceProperty(FieldPropertyInfo propertyInfo, CodeTypeMemberCollection members)
		{
			Assert.ArgumentNotNull(propertyInfo, "propertyInfo");

			var property = new CodeMemberProperty
			{
				Type = new CodeTypeReference(propertyInfo.FieldType.PublicFieldType),
				Name = propertyInfo.FieldPropertyName,
				HasGet = true
			};

			// add [IndexField] attribute
			property.CustomAttributes.Add(GetIndexFieldAttribute(propertyInfo.SearchFieldName));

			AddCommentsToFieldProperty(property, propertyInfo.Field);

			members.Add(property);
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
				Type = new CodeTypeReference(typeof(ID)),
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
				ReturnType = new CodeTypeReference(typeof(IStandardTemplateItem))
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

			attribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_synthesisVersion.ToString(2))));

			type.CustomAttributes.Add(attribute);
		}

		private CodeAttributeDeclaration GetIndexFieldAttribute(string indexFieldName)
		{
			// this is effectively [IndexField("index_field_name")]
			return new CodeAttributeDeclaration(new CodeTypeReference(typeof(IndexFieldAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression(indexFieldName)));
		}

		/// <summary>
		/// Adds the versioning control attribute to the interface
		/// </summary>
		/// <example>[RepresentsSitecoreTemplate("{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}", "Dn8cOiiO0ckeD/NPjd9Q8nJuPSk=")]</example>
		private void AddVersionAttributeToInterface(CodeTypeDeclaration interfaceType, ITemplateInfo template)
		{
			var attribute = new CodeAttributeDeclaration(new CodeTypeReference(typeof(RepresentsSitecoreTemplateAttribute)));

			attribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(template.TemplateId.ToString())));
			attribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_signatureProvider.GenerateTemplateSignature(template))));
			attribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(_parameters.ConfigurationName)));

			interfaceType.CustomAttributes.Add(attribute);
		}

		/// <summary>
		/// Writes a file to disk and creates backup copies if a file with the same name already exists
		/// </summary>
		/// <remarks>Backups are numbered i.e. .1, .2, .3, .4, etc and rotate up as new copies are made (i.e. if .1 exists, it will be renamed .2 and the current one renamed to .1, etc)</remarks>
		private void WriteFileWithBackups(string path, CodeCompileUnit code)
		{
			if (File.Exists(path)) // existing version present; make backups as necessary
			{
				uint maxBackups = _parameters.MaxBackupCopies;

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

			var rawCode = code.CompileToCSharpSourceCode();

			rawCode = Regex.Replace(rawCode, "Runtime Version[^\r]+", string.Empty);

			// write new file
			File.WriteAllText(path, rawCode);
		}

		private CodeCompileUnit CreateCodeCompileUnit()
		{
			var codeUnit = new CodeCompileUnit();

			// required library references to compile
			codeUnit.ReferencedAssemblies.Add(_parameters.SitecoreKernelAssemblyPath);
			codeUnit.ReferencedAssemblies.Add(_parameters.SynthesisAssemblyPath);

			return codeUnit;
		}

		private void SortCodeCompileUnit(CodeCompileUnit unit)
		{
			var namespaces = new CodeNamespace[unit.Namespaces.Count];
			unit.Namespaces.CopyTo(namespaces, 0);
			Array.Sort(namespaces, (ns, codeNamespace) => string.Compare(ns.Name, codeNamespace.Name, StringComparison.Ordinal));
			unit.Namespaces.Clear();
			unit.Namespaces.AddRange(namespaces);

			foreach (CodeNamespace ns in unit.Namespaces)
			{
				var types = new CodeTypeDeclaration[ns.Types.Count];
				ns.Types.CopyTo(types, 0);
				Array.Sort(types, (declaration, typeDeclaration) => string.Compare(declaration.Name, typeDeclaration.Name, StringComparison.Ordinal));
				ns.Types.Clear();
				ns.Types.AddRange(types);
			}
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
	}
}
