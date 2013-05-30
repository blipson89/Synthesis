using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Microsoft.CSharp;

namespace Synthesis.Utility
{
	internal static class CodeDomUtility
	{
		/// <summary>
		/// Creates a new namespace within a CodeCompileUnit and returns the namespace
		/// </summary>
		public static CodeNamespace CreateNamespace(this CodeCompileUnit unit, string name)
		{
			var ns = new CodeNamespace(name);
			unit.Namespaces.Add(ns);

			return ns;
		}

		/// <summary>
		/// Creates a new type (CodeTypeDeclaration) in a given namespace and returns the type
		/// </summary>
		public static CodeTypeDeclaration CreateType(this CodeNamespace @namespace, MemberAttributes attributes, string name)
		{
			var type = new CodeTypeDeclaration(name)
			           	{
			           		Attributes = attributes
			           	};

			@namespace.Types.Add(type);

			return type;
		}

		/// <summary>
		/// Creates a method in a type (CodeTypeDeclaration) and returns the method
		/// </summary>
		public static CodeMemberMethod CreateMethod(this CodeTypeDeclaration type, MemberAttributes attributes, Type returnType, string name, params CodeParameterDeclarationExpression[] parameters)
		{
			var method = new CodeMemberMethod();
			method.Name = name;
			if(returnType != null) method.ReturnType = new CodeTypeReference(returnType);
			method.Attributes = attributes;
			if (parameters != null)
				method.Parameters.AddRange(parameters);

			type.Members.Add(method);

			return method;
		}

		/// <summary>
		/// Compiles an assembly from a CodeDOM CodeCompileUnit
		/// </summary>
		public static CompilerResults CompileCSharp(this CodeCompileUnit code, string outputPath)
		{
			var providerOptions = new Dictionary<string, string>();
			if(System.Environment.Version.Major < 4) providerOptions.Add("CompilerVersion", "v3.5");

			var compiler = new CSharpCodeProvider(providerOptions);

			CompilerResults results = compiler.CompileAssemblyFromDom(GetCompilerParameters(outputPath), code);
			
			if (results.Errors.Count > 0)
			{
				// Display compilation errors.
				var sb = new StringBuilder();
				foreach (CompilerError ce in results.Errors)
				{
					sb.AppendLine(ce.ToString());
				}

				throw new CompileException(sb.ToString(), results.Errors);
			}

			return results;
		}

		/// <summary>
		/// Converts a unit of CodeDOM code into the C# source equivalent
		/// </summary>
		public static string CompileToCSharpSourceCode(this CodeCompileUnit code)
		{
			var providerOptions = new Dictionary<string, string>();
			if (System.Environment.Version.Major < 4) providerOptions.Add("CompilerVersion", "v3.5");

			var compiler = new CSharpCodeProvider(providerOptions);
			string result;

			using(var sourceStream = new MemoryStream())
			{
				var tw = new IndentedTextWriter(new StreamWriter(sourceStream), "\t");

				compiler.GenerateCodeFromCompileUnit(code, tw, new CodeGeneratorOptions());

				tw.Flush();

				sourceStream.Seek(0, SeekOrigin.Begin);
				result = new StreamReader(sourceStream).ReadToEnd();
			}

			return result;
		}

		public static CompilerParameters GetCompilerParameters(string outputPath)
		{
			var parameters = new CompilerParameters();

			parameters.GenerateInMemory = true;
			parameters.GenerateExecutable = false;
			parameters.IncludeDebugInformation = false;
			parameters.OutputAssembly = outputPath;
			return parameters;
		}
	}

	/// <summary>
	/// An exception thrown when a CodeDOM compilation generates errors
	/// </summary>
	[Serializable]
	public class CompileException : Exception
	{
		public CompileException() : base() { }
		public CompileException(string message) : base(message) { }
		public CompileException(string message, Exception innerException) : base(message, innerException) { }
		protected CompileException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public CompileException(string message, CompilerErrorCollection errors)
			: base(message)
		{
			CompilerErrors = errors;
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		public CompilerErrorCollection CompilerErrors { get; private set; }
	}
}
