$moduleDirectory = Resolve-Path -Path (Join-Path -Path $(Get-RootDirectory) -ChildPath "tools\scripts\modules")

function Load-Module([string] $moduleName) {
	$modulePath = Join-Path -Path $moduleDirectory -ChildPath $moduleName
	Write-Host "Loading module: $modulePath" -ForegroundColor yellow
	import-module $modulePath -Force
}

Load-Module "paths.psm1"
Load-Module "msbuild.psm1"