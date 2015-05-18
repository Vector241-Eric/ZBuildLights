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

	New-Package -ProjectName 'ZBuildLights.Web' -BuildConfiguration $localDeploymentSettings.BuildConfiguration -PublishProfile 'Package'
	$projectPath = Get-ProjectDirectory -ProjectName 'ZBuildLights.Web'

	$packageDirectory = $packageSettings.PackageDirectory

	Initialize-Directory $packageSettings.ReleaseDirectory

	$zipFilePath = Get-UniqueZipFilePath

	ZipFiles $zipFilePath $packageDirectory
	Write-Host "Release created at: $zipFilePath" -ForegroundColor Cyan
}