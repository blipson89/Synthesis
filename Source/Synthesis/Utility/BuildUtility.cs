using Microsoft.Build.Evaluation;
using Microsoft.Build.Logging;


namespace Synthesis.Utility
{
	/// <summary>
	/// Allows the ability to programmatically invoke msbuild on a project and compile it
	/// </summary>
	/*
	 * IMPORTANT: You may need to add this assembly redirect to your config file's <runtime> section if you get errors
	 * referring to an inability to build because some .NET Entity Framework targets won't build. The redirect is not required 
	 * if you're using .NET 4. This code requires some refactoring to work with .NET 4.0 as the build target.
		<assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
		  <dependentAssembly>
			  <assemblyIdentity name="Microsoft.Build.Framework" publicKeyToken="b03f5f7f11d50a3a" culture="neutral"/>
			  <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="3.5.0.0"/>
		  </dependentAssembly>
		  <dependentAssembly>
			  <assemblyIdentity name="Microsoft.Build.Engine" publicKeyToken="b03f5f7f11d50a3a" culture="neutral"/>
			  <bindingRedirect oldVersion="0.0.0.0-99.9.9.9" newVersion="3.5.0.0"/>
		  </dependentAssembly>
	  </assemblyBinding>
	*/
	public static class BuildUtility
	{
		/// <summary>
		/// Builds a project (.csproj, .vbproj, etc)
		/// </summary>
		/// <param name="projectFileName">Physical path to project file</param>
		/// <param name="configuration">Configuration to build in (usually "Debug" or "Release")</param>
		/// <param name="logFilePath">Physical path to write a build log file to</param>
		/// <returns>True if the project compiled successfully</returns>
		public static bool BuildProject(string projectFileName, string configuration, string logFilePath)
		{
			return BuildProject(projectFileName, configuration, logFilePath, new [] {"Build"});
		}

		/// <summary>
		/// Builds a project (.csproj, .vbproj, etc)
		/// </summary>
		/// <param name="projectFileName">Physical path to project file</param>
		/// <param name="configuration">Configuration to build in (usually "Debug" or "Release")</param>
		/// <param name="logFilePath">Physical path to write a build log file to</param>
		/// <param name="targets">The build target(s) to build. Usually "Build," "Rebuild," "Clean," etc</param>
		/// <returns>True if the project compiled successfully</returns>
		public static bool BuildProject(string projectFileName, string configuration, string logFilePath, string[] targets)
		{
			var projects = new ProjectCollection(ToolsetDefinitionLocations.Registry);

			var logger = new FileLogger();

			logger.Parameters = @"logfile=" + logFilePath;

			projects.RegisterLogger(logger);
			projects.DefaultToolsVersion = "4.0";
			projects.SetGlobalProperty("Configuration", configuration);

			return projects.LoadProject(projectFileName).Build(targets);
		}
	}
}