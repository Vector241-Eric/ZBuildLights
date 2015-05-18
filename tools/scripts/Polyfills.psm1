$scriptDirectory = Split-Path $script:MyInvocation.MyCommand.Path
$srcDirectory = Resolve-Path -Path (Join-Path -Path $scriptDirectory -ChildPath "..\..\src")

function Get-Project() {
    Param(
        [Parameter(mandatory=$true)] [string]$ProjectName
    )

	$projectDirectory = Join-Path -Path $srcDirectory -ChildPath $ProjectName
	$projectFile = Join-Path -Path $projectDirectory -ChildPath "$($ProjectName).csproj"

	if (-not (Test-Path $projectFile)) {
		throw "Could not locate project file at $projectFile"
	}

	$project = @{}
	$project.FullName = $projectFile
	return $project
}

function Get-RootDirectory() {
	return Resolve-Path -Path (Join-Path -Path $scriptDirectory -ChildPath "..\..")
}

$functionsToExport = @(
	'Get-Project'
	'Get-RootDirectory'
)

Export-ModuleMember -Function $functionsToExport