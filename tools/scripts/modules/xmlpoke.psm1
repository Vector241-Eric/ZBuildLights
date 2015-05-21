function Edit-XML($filePath, $xpath, $value) { 
	Write-Host "Poking [$filePath] - [$xpath] with [$value]"
    [xml] $fileXml = Get-Content $filePath 
    $node = $fileXml.SelectSingleNode($xpath) 
    if ($node) { 
        $node.Value = $value 

        $fileXml.Save($filePath)  
    } 
}

$functionsToExport = @(
	'Edit-XML'
)

Export-ModuleMember -Function $functionsToExport