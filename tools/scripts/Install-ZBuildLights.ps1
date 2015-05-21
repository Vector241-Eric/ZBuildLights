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

#
# Deploy the web application
#	
Write-Host "Resetting deployment directory" -ForegroundColor Cyan
Reset-Directory $zBuildLightsDeploymentSettings.DeploymentPath
Copy-Item "$applicationPath\*" $zBuildLightsDeploymentSettings.DeploymentPath -Recurse
	
Write-Host "Setting up application pool" -ForegroundColor Cyan
Create-AppPool $zBuildLightsDeploymentSettings.AppPool
	
Write-Host "Creating web site" -ForegroundColor Cyan
Create-WebSite $zBuildLightsDeploymentSettings.WebSiteName $zBuildLightsDeploymentSettings.DeploymentPath $zBuildLightsDeploymentSettings.Port

Write-Host "Website created at http://localhost:$($zBuildLightsDeploymentSettings.Port)" -ForegroundColor Yellow


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
#	Copy the binaries from the package to Program Files (x86)
#
Write-Host "Copying binaries from package..." -ForegroundColor Cyan

$servicePackage = Resolve-Path -Path (Join-Path -Path $scriptDirectory -ChildPath "WindowsService")

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