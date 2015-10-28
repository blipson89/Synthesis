param($scriptRoot)

$msBuild = "$env:WINDIR\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
$nuGet = "$scriptRoot..\Dependencies\NuGet.exe"
$solution = "$scriptRoot\..\Synthesis.sln"

& $nuGet restore $solution
& $msBuild $solution /p:Configuration=Release /t:Rebuild /m

$synthesisAssembly = Get-Item "$scriptRoot\..\Source\Synthesis\bin\Release\Synthesis.dll" | Select-Object -ExpandProperty VersionInfo
$targetAssemblyVersion = $synthesisAssembly.ProductVersion

& $nuGet pack "$scriptRoot\Synthesis.nuget\Synthesis.nuspec" -version $targetAssemblyVersion

& $nuGet pack "$scriptRoot\..\Source\Synthesis\Synthesis.csproj" -Symbols -Prop Configuration=Release

& $nuGet pack "$scriptRoot\..\Source\Synthesis.Testing\Synthesis.Testing.csproj" -Symbols -Prop Configuration=Release

& $nuGet pack "$scriptRoot\Synthesis.Mvc.nuget\Synthesis.Mvc.nuspec" -version $targetAssemblyVersion

& $nuGet pack "$scriptRoot\..\Source\Synthesis.Mvc\Synthesis.Mvc.csproj" -Symbols -Prop Configuration=Release