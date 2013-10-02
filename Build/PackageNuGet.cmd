@ECHO off

SET scriptRoot=%~dp0
SET msbuild=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe

"%msbuild%" "%scriptroot%\..\Synthesis.sln" /p:Configuration=Release /m

"%scriptRoot%\..\.nuget\NuGet.exe" pack "%scriptRoot%\Synthesis.nuget\Synthesis.nuspec"

"%scriptRoot%\..\.nuget\NuGet.exe" pack "%scriptRoot%\Synthesis.Core.nuget\Synthesis.Core.nuspec" -Symbols

"%scriptRoot%\..\.nuget\NuGet.exe" pack "%scriptRoot%\Synthesis.Testing.nuget\Synthesis.Testing.nuspec" -Symbols

"%scriptRoot%\..\.nuget\NuGet.exe" pack "%scriptRoot%\Synthesis.Blade.nuget\Synthesis.Blade.nuspec"

"%scriptRoot%\..\.nuget\NuGet.exe" pack "%scriptRoot%\Synthesis.Blade.Core.nuget\Synthesis.Blade.Core.nuspec" -Symbols

PAUSE