function Get-RootDirectory() {
	$markerProject = Split-Path -Path (Get-Project "ZBuildLights.Web").FullName -Parent
	return Resolve-Path -Path (Join-Path -Path $markerProject  -ChildPath "..\..")
}

$functionsToExport = @(
	'Get-RootDirectory'
)

Export-ModuleMember -Function $functionsToExport