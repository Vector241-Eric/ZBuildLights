Write-Host
Write-Host "Installing web application to IIS" -ForegroundColor Yellow
Write-Host

$scriptDirectory = Split-Path $script:MyInvocation.MyCommand.Path
$toolsDirectory = Resolve-Path -Path (Join-Path -Path $scriptDirectory -ChildPath "Powershell\tools")
$applicationPath = Resolve-Path -Path (Join-Path -Path $scriptDirectory -ChildPath "Web")

# Load Modules
import-module (Join-Path -Path $toolsDirectory -ChildPath "scripts\modules\enumerables.psm1") -Force
import-module (Join-Path -Path $toolsDirectory -ChildPath "scripts\modules\iis.psm1") -Force
import-module (Join-Path -Path $toolsDirectory -ChildPath "scripts\modules\file_utilities.psm1") -Force

# Load Deployment Settings
. (Resolve-Path -Path (Join-Path -Path $scriptDirectory -ChildPath "DeploymentSettings.ps1"))
	
Write-Host "Resetting deployment directory" -ForegroundColor Cyan
Reset-Directory $zBuildLightsDeploymentSettings.DeploymentPath
Copy-Item "$applicationPath\*" $zBuildLightsDeploymentSettings.DeploymentPath -Recurse
	
Write-Host "Setting up application pool" -ForegroundColor Cyan
Create-AppPool $zBuildLightsDeploymentSettings.AppPool
	
Write-Host "Creating web site" -ForegroundColor Cyan
Create-WebSite $zBuildLightsDeploymentSettings.WebSiteName $zBuildLightsDeploymentSettings.DeploymentPath $zBuildLightsDeploymentSettings.Port

Write-Host "Website created at http://localhost:$($zBuildLightsDeploymentSettings.Port)" -ForegroundColor Yellow