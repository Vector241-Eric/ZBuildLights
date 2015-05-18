function global:Install-ZBuildLights() {
	Build-Project -ProjectName 'ZBuildLights.Web' -BuildConfiguration $localDeploymentSettings.BuildConfiguration
	$projectPath = Get-ProjectDirectory -ProjectName 'ZBuildLights.Web'
	Install-WebApplication $projectPath
}