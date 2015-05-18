$global:packageSettings = @{}

$packageSettings.PackageDirectory = Join-Path -Path (Get-RootDirectory) -ChildPath "PackageWorkingDirectory\ZBuildLightsPackage"
$packageSettings.ReleaseDirectory = Join-Path -Path (Get-RootDirectory) -ChildPath "Releases"