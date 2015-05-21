function global:Package-ZBuildLights() {
	function ZipFiles( $zipfilename, $sourcedir )
	{
	   Add-Type -Assembly System.IO.Compression.FileSystem
	   $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
	   [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir,
			$zipfilename, $compressionLevel, $false)
	}

	function Get-UniqueZipFilePath() {
		$zipFileName = "ZBuildLights-Release_$((Get-Date).ToString('yyyy-MM-dd')).zip"
		$zipFilePath = Join-Path -Path $packageSettings.ReleaseDirectory -ChildPath $zipFileName
		$counter = 0
		while (Test-Path $zipFilePath) {
			$counter = $counter + 1
			$zipFileName = "ZBuildLights-Release_$((Get-Date).ToString('yyyy-MM-dd'))_$counter.zip"
			$zipFilePath = Join-Path -Path $packageSettings.ReleaseDirectory -ChildPath $zipFileName
		}
		return $zipFilePath
	}
	
	$packageDirectory = $packageSettings.PackageDirectory
	Reset-Directory $packageDirectory

	#
	#	Package the web project
	#
	New-Package -ProjectName $packageSettings.WebProject -BuildConfiguration $packageSettings.WebBuildConfiguration -PublishProfile 'Package'

	#
	#	Package the service project
	#
	Invoke-ProjectBuild -ProjectName $packageSettings.ServiceProject -BuildConfiguration $packageSettings.ServiceBuildConfiguration
	$serviceProjectOutput = Resolve-Path -Path (Join-Path -Path (Get-ProjectDirectory $packageSettings.ServiceProject) -ChildPath "bin\x86\$($packageSettings.ServiceBuildConfiguration)")
	$servicePackageDirectory = "$packageDirectory\WindowsService"
	Copy-Item -Path $serviceProjectOutput -Destination $servicePackageDirectory -Recurse

	$packagedScriptsDirectory = Join-Path -Path $packageDirectory -ChildPath "Powershell"
	Reset-Directory $packagedScriptsDirectory


	#
	#	Copy README into package
	#
	Copy-Item -Path (Join-Path -Path (Get-RootDirectory) -ChildPath "README.md") -Destination $packageDirectory


	#
	#	Copy over PS scripts for installation
	#
	Copy-Item -Path (Join-Path -Path (Get-RootDirectory) -ChildPath "tools\scripts\Install-ZBuildLights.ps1") -Destination $packageDirectory
	Copy-Item -Path (Join-Path -Path (Get-RootDirectory) -ChildPath "tools\scripts\DeploymentSettings.ps1") -Destination $packageDirectory
	Copy-Item -Path (Join-Path -Path (Get-RootDirectory) -ChildPath "tools") -Destination $packagedScriptsDirectory -Recurse

	Initialize-Directory $packageSettings.ReleaseDirectory

	$zipFilePath = Get-UniqueZipFilePath

	ZipFiles $zipFilePath $packageDirectory
	Write-Host "Release created at: $zipFilePath" -ForegroundColor Cyan
}