function global:Install-WebApplication([string] $applicationPath) {
	Write-Notification "Installing web application from '$applicationPath'"

	#
	#	Helper Functions
	#
	$appcmd = "$($env:windir)\system32\inetsrv\appcmd.exe"

	function Create-AppPool([string] $appPoolName) {
		$appPools = Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd list apppool"))

		[bool] $hasAppPool = 
			$appPools -split [environment]::NewLine | Test-Any {($_ -split '"')[1].ToUpper() -eq $appPoolName.ToUpper() }

		Write-Host "Making sure IIS application pool [$appPoolName] is ready..."
		if ($hasAppPool) {
			Write-Host "    Application pool will be reset."
			Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd recycle apppool /apppool.name:$appPoolName"))
		}
		else {
			Write-Host "    Application pool will be created."
			Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd add apppool /name:$appPoolName /managedRuntimeVersion:v4.0"))
			Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd set apppool /apppool.name:$appPoolName -enable32BitAppOnWin64:true"))
		}
		Write-Host "Application pool setup complete."
	}
	
	function Create-WebSite([string] $webSite, [string] $deployPath, [string] $portNumber) {
		$webSites = Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd list site"))

		[bool] $hasWebSite = 
			$webSites -split [environment]::NewLine  | Test-Any {($_ -split '"')[1].ToUpper() -eq $webSite.ToUpper()}

		if ($hasWebSite) {
			Write-Host "Web site $webSiteName already exists"
		}
		else {
			Write-Host "New Web Site: '$webSite'"
			Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd add site /name:$webSite /physicalPath:`"$deployPath`" /bindings:http/*:$($portNumber):"))
			Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd set app $webSite/ /applicationPool:$($localDeploymentSettings.AppPool)"))
			Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd start site `"$webSite`""))
		}
	}

	function Create-Application([string] $webSite, [string] $appName) {
		$applications = Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd list app"))
		$fullAppName = "$webSite/$appName".ToUpper()

		[bool] $hasApplication = 
			$applications -split [environment]::NewLine  | Test-Any {($_ -split '"')[1].ToUpper() -eq $fullAppName}

		if ($hasApplication) {
			Write-Host "Found existing application $fullAppName"
		}
		else {
			$addScript = "$appcmd add app /site.name:$webSite /path:/$appName /physicalPath:$($localDeploymentSettings.DeploymentPath)"
			
			Write-Host "Add App [$addScript]"
			Invoke-Command -ScriptBlock ([scriptblock]::Create($addScript))
			Write-Host "Set App"
			Invoke-Command -ScriptBlock ([scriptblock]::Create("$appcmd set app $fullAppName /applicationPool:$($localDeploymentSettings.AppPool)"))
		}
	}


	#
	#	Main Script Body
	#
	Write-Notification "Installing web application to IIS"
	
	Write-Notification "Resetting deployment directory"
	Reset-Directory $localDeploymentSettings.DeploymentPath
	Copy-Item "$applicationPath\*" $localDeploymentSettings.DeploymentPath -Recurse
	
	Write-Notification "Setting up application pool"
	Create-AppPool $localDeploymentSettings.AppPool
	
	Write-Notification "Creating web site"
	Create-WebSite $localDeploymentSettings.WebSiteName $localDeploymentSettings.DeploymentPath $localDeploymentSettings.Port
	
#	Write-Notification "Creating web application"
#	Create-Application $localDeploymentSettings.WebSiteName $localDeploymentSettings.WebApplicationName
}