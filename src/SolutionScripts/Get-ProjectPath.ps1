#
#	This implementation only works in Package Manager Console
#
Function global:Get-PathToProjectFile([string]$projectName) {
	return (Get-Project $projectName).FullName
}
