$msbuildExe = "C:\Program Files (x86)\MSBuild\12.0\bin\MSBuild.exe"

function Invoke-MsBuild([string]$commandlineOptions) {
	if (-not (Test-Path $msbuildExe)) {
		throw "Could not locate MSBUILD at '$msbuildExe'.  Please set the msbuildExe variable to the correct path (in this .ps1 file)."
	}

    $msbuildCommand = "& `"$msbuildExe`" $commandlineOptions"

    Write-Host
    Write-Host "MSBUILD [$msbuildCommand]" -ForegroundColor Yellow
    Write-Host

    Invoke-Expression $msbuildCommand | Write-Host
    Write-Host
    $success = $LASTEXITCODE -eq 0

    if (-not $success) {
        throw "MSBUILD Failed."
    }

    return $success
}

function Get-PathToProjectFile([string]$projectName) {
	return (Get-Project $projectName).FullName
}

function Get-ProjectDirectory() {
    Param(
        [Parameter(mandatory=$true)] [string]$ProjectName
    )

    $projectFile = Get-PathToProjectFile $ProjectName
	return Split-Path -Path $projectFile -Parent
}

function New-Package() {
    Param(
        [Parameter(mandatory=$true)] [string]$ProjectName,
        [Parameter(mandatory=$true)] [string]$BuildConfiguration,
        [Parameter(mandatory=$true)] [string]$PublishProfile
    )

    $projectFile = Get-PathToProjectFile $ProjectName
	$verbosity = "`"/verbosity:minimal`""
	$config = "`"/property:Configuration=$BuildConfiguration`""
	$target = "`"/target:rebuild`""
	$platform = "`"/property:Platform=x86`""
	$package = "`"/property:DeployOnBuild=true`" `"/property:PublishProfile=$PublishProfile`""

    $buildSuccess = Invoke-MsBuild "`"$projectFile`" $verbosity $target $config $platform $package"
}

function Invoke-ProjectBuild() {
    Param(
        [Parameter(mandatory=$true)] [string]$ProjectName,
        [Parameter(mandatory=$true)] [string]$BuildConfiguration
    )

    $projectFile = Get-PathToProjectFile $ProjectName
    $buildSuccess = Invoke-MsBuild "`"$projectFile`" `"/verbosity:minimal`" `"/target:rebuild`" `"/property:Configuration=$BuildConfiguration`" `"/property:Platform=x86`""
}

$functionsToExport = @(
	'Build-Project'
	'Get-ProjectDirectory'
	'Invoke-MsBuild'
	'Invoke-ProjectBuild'
	'New-Package'
)

Export-ModuleMember -Function $functionsToExport