$global:packageSettings = @{}

$packageSettings.PackageDirectory = Join-Path -Path (Get-RootDirectory) -ChildPath "PackageWorkingDirectory\ZBuildLightsPackage"
$packageSettings.ReleaseDirectory = Join-Path -Path (Get-RootDirectory) -ChildPath "Releases"

$packageSettings.WebProject = "ZBuildLights.Web"
$packageSettings.WebBuildConfiguration = "Release"

$packageSettings.ServiceProject = "ZBuildLightsUpdater"
$packageSettings.ServiceBuildConfiguration = "Release"
$packageSettings.ServicePublishDirectory = "PackageWorkingDirectory\WindowsService"
