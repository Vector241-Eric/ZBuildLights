function global:Reset-Directory([string] $directoryPath) {
	if (Test-Path $directoryPath) {
		Remove-Item $directoryPath -Recurse -ErrorAction SilentlyContinue
	}
	New-Item -ItemType directory -Path $directoryPath
}

function global:Initialize-Directory([string] $directoryPath) {
	if (-not (Test-Path $directoryPath)) {
		New-Item -ItemType directory -Path $directoryPath
	}
}

$functionsToExport = @(
	'Initialize-Directory'
	'Reset-Directory'
)

Export-ModuleMember -Function $functionsToExport