$global:msbuildExe = "C:\Program Files (x86)\MSBuild\12.0\bin\MSBuild.exe"

function global:Invoke-MsBuild([string]$commandlineOptions) {
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

function global:Get-ProjectBinaryDirectory() {
    Param(
        [Parameter(mandatory=$true)] [string]$ProjectName,
        [Parameter(mandatory=$true)] [string]$BuildConfiguration
    )

    $projectFile = Get-PathToProjectFile $ProjectName
	$projectDirectory = Split-Path -Path $projectFile -Parent
	return "$projectDirectory\bin\$BuildConfiguration"
}

function global:Get-ProjectDirectory() {
    Param(
        [Parameter(mandatory=$true)] [string]$ProjectName
    )

    $projectFile = Get-PathToProjectFile $ProjectName
	return Split-Path -Path $projectFile -Parent
}

function global:Build-Project() {
    Param(
        [Parameter(mandatory=$true)] [string]$ProjectName,
        [Parameter(mandatory=$true)] [string]$BuildConfiguration
    )

    $projectFile = Get-PathToProjectFile $ProjectName
    $buildSuccess = Invoke-MsBuild "`"$projectFile`" `"/verbosity:minimal`" `"/target:rebuild`" `"/property:Configuration=$BuildConfiguration`" `"/property:Platform=x86`""
}