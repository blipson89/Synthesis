param(
	[string]$scriptRoot,
	[string]$Mode = "Release"
)

$ErrorActionPreference = "Stop"

function Resolve-MsBuild {
	$msb2017 = Resolve-Path "${env:ProgramFiles(x86)}\Microsoft Visual Studio\*\*\MSBuild\*\bin\msbuild.exe" -ErrorAction SilentlyContinue
	if($msb2017) {
		Write-Host "Found MSBuild 2017 (or later)."
		Write-Host $msb2017
		return $msb2017
	}

	$msBuild2015 = "${env:ProgramFiles(x86)}\MSBuild\14.0\bin\msbuild.exe"

	if(-not (Test-Path $msBuild2015)) {
		throw 'Could not find MSBuild 2015 or later.'
	}

	Write-Host "Found MSBuild 2015."
	Write-Host $msBuild2015

	return $msBuild2015
}

$msBuild = Resolve-MsBuild
$nuGet = "$scriptRoot..\Dependencies\NuGet.exe"
$solution = "$scriptRoot\..\Synthesis.sln"

dotnet-gitversion /updateassemblyinfo Source/SharedAssemblyInfo.cs

& $nuGet restore $solution
& $msBuild $solution /p:Configuration=$Mode /t:Rebuild /m

$synthesisAssembly = Get-Item "$scriptRoot\..\Source\Synthesis\bin\$Mode\*\Synthesis.dll" | Select-Object -ExpandProperty VersionInfo

$targetAssemblyVersion = $synthesisAssembly.ProductVersion

& $nuGet pack "$scriptRoot\Synthesis.nuget\Synthesis.nuspec" -version $targetAssemblyVersion -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"

& $nuGet pack "$scriptRoot\..\Source\Synthesis\Synthesis.csproj" -Symbols -Prop Configuration=$Mode -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"

& $nuGet pack "$scriptRoot\..\Source\Synthesis.Testing\Synthesis.Testing.csproj" -Symbols -Prop Configuration=$Mode -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"

& $nuGet pack "$scriptRoot\Synthesis.Mvc.nuget\Synthesis.Mvc.nuspec" -version $targetAssemblyVersion -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"

& $nuGet pack "$scriptRoot\..\Source\Synthesis.Mvc\Synthesis.Mvc.csproj" -Symbols -Prop Configuration=$Mode -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"

& $nuGet pack "$scriptRoot\Synthesis.Solr.nuget\Synthesis.Solr.nuspec" -version $targetAssemblyVersion -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"

& $nuGet pack "$scriptRoot\..\Source\Synthesis.Solr\Synthesis.Solr.csproj" -Symbols -Prop Configuration=$Mode -OutputDirectory "$scriptRoot\Versions\$targetAssemblyVersion"