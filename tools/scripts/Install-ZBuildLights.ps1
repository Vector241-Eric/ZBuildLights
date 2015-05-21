Write-Host
Write-Host "Installing web application to IIS" -ForegroundColor Yellow
Write-Host

$scriptDirectory = Split-Path $script:MyInvocation.MyCommand.Path
$toolsDirectory = Resolve-Path -Path (Join-Path -Path $scriptDirectory -ChildPath "Powershell\tools")
$webPackage = Resolve-Path -Path (Join-Path -Path $scriptDirectory -ChildPath "Web")
$servicePackage = Resolve-Path -Path (Join-Path -Path $scriptDirectory -ChildPath "WindowsService")


#
# Load Modules
#
import-module (Join-Path -Path $toolsDirectory -ChildPath "scripts\modules\enumerables.psm1") -Force
import-module (Join-Path -Path $toolsDirectory -ChildPath "scripts\modules\iis.psm1") -Force
import-module (Join-Path -Path $toolsDirectory -ChildPath "scripts\modules\file_utilities.psm1") -Force
import-module (Join-Path -Path $toolsDirectory -ChildPath "scripts\modules\xmlpoke.psm1") -Force


#
# Load Deployment Settings
#
. (Resolve-Path -Path (Join-Path -Path $scriptDirectory -ChildPath "DeploymentSettings.ps1"))


#
# Poke the configuration file
#
$webConfigurationFile = Resolve-Path -Path (Join-Path -Path $webPackage -ChildPath "web.config")
Edit-XML $webConfigurationFile "/configuration/appSettings/add[@key='ZWaveControllerComPort']/@value" $zBuildLightsDeploymentSettings.ZWaveControllerComPort
Edit-XML $webConfigurationFile "/configuration/appSettings/add[@key='StorageFilePath']/@value" $zBuildLightsDeploymentSettings.StorageFilePath
Edit-XML $webConfigurationFile "/configuration/appSettings/add[@key='ZWaveConfigurationPath']/@value" $zBuildLightsDeploymentSettings.ZWaveConfigurationPath


#
# Poke the NLog.configs
#
$webNLog = Resolve-Path -Path (Join-Path -Path $webPackage -ChildPath "NLog.config")
$serviceNLog = Resolve-Path -Path (Join-Path -Path $servicePackage -ChildPath "NLog.config")
$targetXPath = "/*[local-name()='nlog']/*[local-name()='targets']/*[local-name()='target']/*[local-name()='target']/@fileName"
$webLogPathSetting = "$($zBuildLightsDeploymentSettings.WebLogsPath)\`${shortdate}-Trace.log"
$serviceLogPathSetting = "$($zBuildLightsDeploymentSettings.ServiceLogsPath)\`${shortdate}-Trace.log"
Edit-XML $webNLog $targetXPath $webLogPathSetting
Edit-XML $serviceNLog $targetXPath $serviceLogPathSetting


#
#	Make sure some directories exist
#
$storageDirectory = Split-Path -Path $zBuildLightsDeploymentSettings.StorageFilePath -Parent
if (-not (Test-Path $storageDirectory)) {
	New-Item -ItemType directory -Path $storageDirectory
}

if (-not (Test-Path $zBuildLightsDeploymentSettings.ZWaveConfigurationPath)) {
	New-Item -ItemType directory -Path $zBuildLightsDeploymentSettings.ZWaveConfigurationPath
}


#
# Deploy the web application
#	
Write-Host "Resetting deployment directory" -ForegroundColor Cyan
Reset-Directory $zBuildLightsDeploymentSettings.DeploymentPath
Copy-Item "$webPackage\*" $zBuildLightsDeploymentSettings.DeploymentPath -Recurse
	
Write-Host "Setting up application pool" -ForegroundColor Cyan
New-AppPool $zBuildLightsDeploymentSettings.AppPool
	
Write-Host "Creating web site" -ForegroundColor Cyan
New-WebSite $zBuildLightsDeploymentSettings.WebSiteName $zBuildLightsDeploymentSettings.DeploymentPath $zBuildLightsDeploymentSettings.Port

$websiteAddress = "http://localhost:$($zBuildLightsDeploymentSettings.Port)"

Write-Host "Website created at $websiteAddress" -ForegroundColor Yellow


#################################################################
#	Service Installation
#################################################################
Write-Host 
Write-Host "Installing ZBuildLights Service" -ForegroundColor Yellow
Write-Host

#
#	Determine if an existing service should be removed
#
$serviceName = "ZBuildLights Updater"
$installUtil = Resolve-Path -Path "${Env:SystemRoot}\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe"
$programFiles86 = Resolve-Path -Path "${Env:ProgramFiles(x86)}"
$programFilesServiceDirectory = Join-Path -Path $programFiles86 -ChildPath "ZBuildLightsService"
$serviceExe = Join-Path -Path $programFilesServiceDirectory -ChildPath "ZBuildLightsUpdater.exe"

Write-Host "Service exe path is: $serviceExe"

$scCmd = Resolve-Path -Path "${Env:SystemRoot}\System32\sc.exe"
$installedServices = (Invoke-Command -ScriptBlock ([scriptblock]::Create("$scCmd query"))) | 
	Where-Object {$_ -like "*SERVICE_NAME*"} | 
	Select-Object @{Name="Service"; Expression={($_ -split ': ')[1]}} | 
	% {$_.Service.ToUpper()}

$requiresUninstallOldService = $FALSE
$searchName = $serviceName.ToUpper()
foreach ($s in $installedServices) {
	$check = $s.ToUpper()
	if ($check -eq $searchName) {
		$requiresUninstallOldService = $TRUE
	}
}

if ($requiresUninstallOldService) {
	Write-Host "Uninstalling existing service from [$serviceExe]" -ForegroundColor Yellow
	$resolvedExePath = Resolve-Path -Path $serviceExe
	& $installUtil "/u" $resolvedExePath
} else {
	Write-Host "No need to remove any existing services."
}

#
#	Modify the configuration to match the deployed site
#
$serviceConfigFile = Resolve-Path -Path (Join-Path -Path $servicePackage -ChildPath "ZBuildLightsUpdater.exe.config")

Edit-XML $serviceConfigFile "/configuration/appSettings/add[@key='TriggerUrl']/@value" "$websiteAddress/Home/UpdateLights"
Edit-XML $serviceConfigFile "/configuration/appSettings/add[@key='UpdateIntervalSeconds']/@value" $zBuildLightsDeploymentSettings.UpdateIntervalSeconds

#
#	Copy the binaries from the package to Program Files (x86)
#
Write-Host "Copying binaries from package..." -ForegroundColor Cyan


if (Test-Path $programFilesServiceDirectory) {
	Remove-Item $programFilesServiceDirectory -Recurse
}
Write-Host "Copying [$servicePackage] to [$programFilesServiceDirectory]"
Copy-Item -Path $servicePackage -Destination $programFilesServiceDirectory -Recurse

#
#	Install the service
#
Write-Host "Installing the service..." -ForegroundColor Cyan
$netUtil = Resolve-Path -Path "${Env:SystemRoot}\system32\net.EXE"
$resolvedExePath = Resolve-Path -Path $serviceExe

& $installUtil $resolvedExePath

#
#	Start the service
#
Write-Host "Starting the ZBuildLights service..." -ForegroundColor Cyan
& $netUtil "start" "ZBuildLights Updater"
Write-Host "Service started" -ForegroundColor Yellow