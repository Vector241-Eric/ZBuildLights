$global:packageSettings = @{}

$packageSettings.PackageDirectory = Join-Path -Path (Get-RootDirectory) -ChildPath "PackageWorkingDirectory\ZBuildLightsPackage"
$packageSettings.ReleaseDirectory = Join-Path -Path (Get-RootDirectory) -ChildPath "Releases"
$packageSettings.Project = "ZBuildLights.Web"
$packageSettings.BuildConfiguration = "Release"