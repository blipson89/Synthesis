param(
    [Parameter(Mandatory=$true)] [string] $version
)
gci "versions\$version\*.nupkg" -exclude *.symbols.nupkg,*.Testing* | % { ..\dependencies\NuGet.exe push $_ -Source https://api.nuget.org/v3/index.json }