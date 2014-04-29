param($scriptRoot)

$msBuild = "$env:WINDIR\Microsoft.NET\Framework\v4.0.30319\msbuild.exe"
$nuGet = "$scriptRoot..\Dependencies\NuGet.exe"
$solution = "$scriptRoot\..\Synthesis.sln"

& $nuGet restore $solution
& $msBuild $solution /p:Configuration=Release /t:Rebuild /m

$synthesisAssembly = Get-Item "$scriptRoot\..\Source\Synthesis\bin\Release\Synthesis.dll" | Select-Object -ExpandProperty VersionInfo
$targetAssemblyVersion = $synthesisAssembly.ProductVersion

& $nuGet pack "$scriptRoot\Synthesis.nuget\Synthesis.nuspec" -version $targetAssemblyVersion

& $nuGet pack "$scriptRoot\..\Source\Synthesis\Synthesis.csproj" -Symbols

& $nuGet pack "$scriptRoot\..\Source\Synthesis.Testing\Synthesis.Testing.csproj" -Symbols

& $nuGet pack "$scriptRoot\Synthesis.Blade.nuget\Synthesis.Blade.nuspec" -version $targetAssemblyVersion

& $nuGet pack "$scriptRoot\..\Source\Synthesis.Blade\Synthesis.Blade.csproj" -Symbols

& $nuGet pack "$scriptRoot\..\Source\Synthesis.Blade.Ninject\Synthesis.Blade.Ninject.csproj" -Symbols