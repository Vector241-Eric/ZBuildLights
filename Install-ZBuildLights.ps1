$scriptDirectory = Split-Path $script:MyInvocation.MyCommand.Path
$toolsDirectory = Join-Path -Path $scriptDirectory -ChildPath "tools"

import-module (Join-Path -Path $toolsDirectory -ChildPath "scripts\Polyfills.psm1") -Force
import-module (Join-Path -Path $toolsDirectory -ChildPath "scripts\modules\paths.psm1") -Force

function Initialize-SolutionScript([string] $scriptName) {
	$solutionScripts = Get-SolutionScriptsDirectory
	$path = Join-Path -Path $solutionScripts -ChildPath $scriptName
	Write-Host "Loading script from [$path]" -ForegroundColor Yellow
	. $path
}

Initialize-SolutionScript "01_PackageSettings.ps1"

#$project = Get-Project "ZBuildLights.Web"

#Write-Host "Project at $($project.FullName)"