param(
	[string]$scriptRoot,
	[string]$Mode = "Release"
)

$ErrorActionPreference = "Stop"
function Resolve-MsBuild {
	$msb2019 = Resolve-Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\*\MSBuild\*\bin\msbuild.exe" -ErrorAction SilentlyContinue
	if($msb2019) {
		return $msb2019
	}
	throw "Unable to find MS Build 2019"
}

$msBuild = Resolve-MsBuild
if ($msBuild -is [array]) {
	$msBuild = $msBuild[1]
}

$nuGet = "$scriptRoot..\Dependencies\NuGet.exe"
$solution = "$scriptRoot\..\Synthesis.sln"
Push-Location (Resolve-Path -Path "$scriptRoot\..")
$gitversion = ConvertFrom-Json ([string](dotnet-gitversion /updateassemblyinfo "Source/SharedAssemblyInfo.cs"))
Pop-Location

& $nuGet restore $solution
& $msBuild $solution /p:Configuration=$Mode /t:Rebuild /m

$targetAssemblyVersion = $gitversion.NuGetVersion

& $nuGet pack "$scriptRoot\Synthesis.nuget\Synthesis.nuspec" -version $targetAssemblyVersion -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"

& $nuGet pack "$scriptRoot\..\Source\Synthesis\Synthesis.csproj" -version $targetAssemblyVersion -Symbols -Prop Configuration=$Mode -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"

& $nuGet pack "$scriptRoot\..\Source\Synthesis.Testing\Synthesis.Testing.csproj" -version $targetAssemblyVersion -Symbols -Prop Configuration=$Mode -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"

& $nuGet pack "$scriptRoot\Synthesis.Mvc.nuget\Synthesis.Mvc.nuspec" -version $targetAssemblyVersion -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"

& $nuGet pack "$scriptRoot\..\Source\Synthesis.Mvc\Synthesis.Mvc.csproj" -version $targetAssemblyVersion -Symbols -Prop Configuration=$Mode -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"

& $nuGet pack "$scriptRoot\Synthesis.Solr.nuget\Synthesis.Solr.nuspec" -version $targetAssemblyVersion -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"

& $nuGet pack "$scriptRoot\..\Source\Synthesis.Solr\Synthesis.Solr.csproj" -version $targetAssemblyVersion -Symbols -Prop Configuration=$Mode -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"