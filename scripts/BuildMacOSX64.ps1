[string] $Product = "LiteCosmosExplorer"
[string] $Target = "MacOSX64"

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target -BuildArgs "-t:BundleApp -p:RuntimeIdentifier=osx-x64 -p:IncludeAllContentForSelfExtract=true" -ProjectPath "CosmosExplorer.Avalonia\CosmosExplorer.Avalonia.csproj"

# Remove everything except the app bundle
Get-Childitem "$PSScriptRoot\..\bld\$Product.$Target\" -Exclude "LiteCosmosExplorer.app" | Remove-Item -Recurse

& "$PSScriptRoot\Build.ps1" -Product $Product -Target $Target -Clean $False -BuildArgs "-t:Publish -p:RuntimeIdentifier=osx-x64 -p:PublishSingleFile=true -p:IncludeAllContentForSelfExtract=true"

# Remove unbundled 
#Remove-Item "$PSScriptRoot\..\bld\$Product.$Target\LiteCosmosExplorer"

& "$PSScriptRoot\TarRelease.ps1" -Product $Product -Target $Target
