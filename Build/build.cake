#addin nuget:?package=Cake.AssemblyInfoReflector&version=1.1.0-beta2
#addin nuget:?package=Mono.Cecil&version=0.10.0

var configuration = Argument("configuration", "Release");

var synthesisVersion = "0.0.0";
Task("Restore-NuGet-Packages").Does(() => {
    NuGetRestore("../Synthesis.sln");
});
Task("Get-Assembly-Version").Does(() => {
    synthesisVersion = ParseAssemblyInfo("../Source/SharedAssemblyInfo.cs").AssemblyInformationalVersion;
});
Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() => {
        MSBuild("../Synthesis.sln", c => c
            .SetConfiguration(configuration)
            .WithTarget("Rebuild")
            .SetMaxCpuCount(null));
    }
);

Task("Pack")
    .IsDependentOn("Build")
    .IsDependentOn("Get-Assembly-Version")
    .Does(() => { 
        // Synthesis and Synthesis.Core
        NuGetPack("Synthesis.nuget/Synthesis.nuspec", new NuGetPackSettings{ Version = synthesisVersion, OutputDirectory=$"Versions/{synthesisVersion}" });
        NuGetPack("../Source/Synthesis/Synthesis.csproj", new NuGetPackSettings{ Symbols = true, OutputDirectory=$"Versions/{synthesisVersion}", Properties = new Dictionary<string, string>{ {"Configuration", "Release"} }});

        // Synthesis.Mvc and Synthesis.Mvc.Core
        NuGetPack("Synthesis.Mvc.nuget/Synthesis.Mvc.nuspec", new NuGetPackSettings{ Version = synthesisVersion, OutputDirectory=$"Versions/{synthesisVersion}" });
        NuGetPack("../Source/Synthesis.Mvc/Synthesis.Mvc.csproj", new NuGetPackSettings{ Symbols = true, OutputDirectory=$"Versions/{synthesisVersion}", Properties = new Dictionary<string, string>{ {"Configuration", "Release"} }});

        // Synthesis.Solr and Synthesis.Solr.Core
        NuGetPack("Synthesis.Solr.nuget/Synthesis.Solr.nuspec", new NuGetPackSettings{ Version = synthesisVersion, OutputDirectory=$"Versions/{synthesisVersion}" });
        NuGetPack("../Source/Synthesis.Solr/Synthesis.Solr.csproj", new NuGetPackSettings{ Symbols = true, OutputDirectory=$"Versions/{synthesisVersion}", Properties = new Dictionary<string, string>{ {"Configuration", "Release"} }});
        
        // Synthesis.Testing
        NuGetPack("../Source/Synthesis.Testing/Synthesis.Testing.csproj", new NuGetPackSettings{ Symbols = true, OutputDirectory=$"Versions/{synthesisVersion}", Properties = new Dictionary<string, string>{ {"Configuration", "Release"} }});
    }
);

RunTarget("Pack");