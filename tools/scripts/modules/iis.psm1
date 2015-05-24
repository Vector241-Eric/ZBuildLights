$appcmd = "$($env:windir)\system32\inetsrv\appcmd.exe"

function New-AppPool([string] $appPoolName) {

	$poolNames = (Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd list apppool"))) -split "APPPOOL " | 
		Where-Object {$_.length -gt 0} | 
		Select-Object @{Name="AppPool"; Expression={($_ -split "`"")[1]}} | 
		% {$_.AppPool.ToUpper()}

	$checkName = $appPoolName.ToUpper()

	[bool] $hasAppPool = $FALSE
	foreach ($p in $poolNames) {
		if ($p -eq $checkName) { $hasAppPool = $TRUE }
	}

	Write-Host "Making sure IIS application pool [$appPoolName] is ready..."
	if ($hasAppPool) {
		Write-Host "    Existing application pool will be reset."
		Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd recycle apppool /apppool.name:$appPoolName"))
	}
	else {
		Write-Host "    Existing application pool will be created."
		Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd add apppool /name:$appPoolName /managedRuntimeVersion:v4.0"))
		Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd set apppool /apppool.name:$appPoolName -enable32BitAppOnWin64:true"))
	}
	Write-Host "Application pool setup complete."
}
	
function New-WebSite([string] $webSite, [string] $deployPath, [string] $portNumber, [string] $appPool) {
	$webSites = (Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd list site"))) -split "SITE " | 
		Where-Object {$_.length -gt 0} | 
		Select-Object @{Name="WebSite"; Expression={($_ -split "`"")[1]}} | 
		% {$_.WebSite.ToUpper()}

	[bool] $hasWebSite = $FALSE
	$checkName = $webSite.ToUpper()
	foreach($site in $webSites) {
		if ($site -eq $checkName) {
			$hasWebSite = $TRUE
		}
	}

	if ($hasWebSite) {
		Write-Host "Web site $webSiteName already exists"
	}
	else {
		Write-Host "New Web Site: '$webSite'"
		Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd add site /name:$webSite /physicalPath:`"$deployPath`" /bindings:http/*:$($portNumber):"))
	}
	Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd set site /site.name:$webSite `"/[path='/'].applicationPool:$appPool`""))
	Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd start site `"$webSite`""))
}


$functionsToExport = @(
	'New-AppPool'
	'New-WebSite'
)

Export-ModuleMember -Function $functionsToExport