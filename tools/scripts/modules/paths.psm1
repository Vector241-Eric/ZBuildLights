function Get-RootDirectory() {
	$markerProject = Split-Path -Path (Get-Project "ZBuildLights.Web").FullName -Parent
	return Resolve-Path -Path (Join-Path -Path $markerProject  -ChildPath "..\..")
}

function Get-SolutionScriptsDirectory() {
	return Resolve-Path -Path (Join-Path -Path (Get-RootDirectory) -ChildPath "src\SolutionScripts")
}

$functionsToExport = @(
	'Get-RootDirectory'
	'Get-SolutionScriptsDirectory'
)

Export-ModuleMember -Function $functionsToExport