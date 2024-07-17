param(
    [string]$Product,
    [string]$Target,
    [boolean]$Clean = $True,
    [string]$BuildArgs = "",
    [string]$ProjectPath = "src\$Product.sln"
)

[string] $RepoRoot = Resolve-Path "$PSScriptRoot\.."

[string] $OutputRoot = "bld"
[string] $TargetOutputDirectory = "$Product.$Target"

$StartingLocation = Get-Location
Set-Location -Path $RepoRoot

if ($Clean -and (Test-Path "$OutputRoot\$TargetOutputDirectory")) {
    Write-Host "Clean output folder..."
    Remove-Item "$OutputRoot\$TargetOutputDirectory" -Recurse
}

Write-Host "Build release..."
try
{
    dotnet msbuild $BuildArgs.Split() -restore -p:Configuration=Release -p:PublishDir="$RepoRoot\$OutputRoot\$TargetOutputDirectory" "$ProjectPath"
    if (!$?) {
    	throw 'Build failed!'
    }
}
finally
{
    Set-Location -Path "$StartingLocation"
}
