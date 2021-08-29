param(
    [Parameter(Mandatory=$true)] [string] $version,
    [Parameter(Mandatory=$true)] [string] $apiKey
)
gci "versions\$version\*.nupkg" -exclude *.symbols.nupkg | % { dotnet nuget push $_.FullName --api-key $apiKey -s https://api.nuget.org/v3/index.json }