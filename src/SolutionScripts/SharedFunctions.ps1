Function global:Write-Notification([string]$m) {
	Write-Host "$([environment]::NewLine)$m" -ForegroundColor Cyan
}