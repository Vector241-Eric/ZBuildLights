$global:zBuildLightsDeploymentSettings = @{}

#######################################
# Web Site Settings
#######################################
$zBuildLightsDeploymentSettings.DeploymentPath = "c:\inetpub\ZBuildLights"
$zBuildLightsDeploymentSettings.AppPool = "ZBuildLights"
$zBuildLightsDeploymentSettings.WebSiteName = "ZBuildLights"
$zBuildLightsDeploymentSettings.Port = "8088"

$zBuildLightsDeploymentSettings.ZWaveControllerComPort = "COM3"
$zBuildLightsDeploymentSettings.StorageFilePath = "c:\var\ZBuildLights\DataModel\MasterModel.json"
$zBuildLightsDeploymentSettings.ZWaveConfigurationPath = "c:\var\ZBuildLights\OpenZWave"


#######################################
# Service Settings
#######################################

# How frequently to query all CI servers for status and update all light displays
$zBuildLightsDeploymentSettings.UpdateIntervalSeconds = "15"

#######################################
# Log Files
#######################################
$zBuildLightsDeploymentSettings.ServiceLogsPath = "c:\var\ZBuildLights\Logs\Service"
$zBuildLightsDeploymentSettings.WebLogsPath = "c:\var\ZBuildLights\Logs\Web"