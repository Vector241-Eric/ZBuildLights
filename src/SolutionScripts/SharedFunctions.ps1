Function global:Write-Notification([string]$m) {
	Write-Host "$([environment]::NewLine)$m" -ForegroundColor Cyan
}

function global:Reset-Directory([string] $directoryPath) {
	if (Test-Path $directoryPath) {
		Remove-Item $directoryPath -Recurse -ErrorAction SilentlyContinue
	}
	New-Item -ItemType directory -Path $directoryPath
}

function global:Test-Any {
    [CmdletBinding()]
    param($EvaluateCondition,
        [Parameter(ValueFromPipeline = $true)] $ObjectToTest)
    begin {
        $any = $false
    }
    process {
        if (-not $any -and (& $EvaluateCondition $ObjectToTest)) {
            $any = $true
        }
    }
    end {
        $any
    }
}