$global:localDeploymentSettings = @{}

$localDeploymentSettings.DeploymentPath = "c:\inetpub\ZBuildLights"
$localDeploymentSettings.AppPool = "ZBuildLights"
$localDeploymentSettings.WebSiteName = "ZBuildLights"
$localDeploymentSettings.WebApplicationName = "ZBuildLights"
$localDeploymentSettings.Port = "8088"
$localDeploymentSettings.Project = "ZBuildLights.Web"
$localDeploymentSettings.BuildConfiguration = "Release"