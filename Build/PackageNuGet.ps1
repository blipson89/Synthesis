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