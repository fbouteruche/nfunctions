function Add-AWSProjectTypeProperty {
    [OutputType([System.Xml.XmlDocument])]
    param(
        [Parameter(Mandatory=$true)]
        [string]$FileName,
        
        [Parameter(Mandatory=$false)]
        [string]$PropertyName = "AWSProjectType",
        
        [Parameter(Mandatory=$false)]
        [string]$PropertyValue = "Lambda"
    )

    $xml = [xml](Get-Content $FileName)

    # Find the first PropertyGroup element (or create one if it doesn't exist)
    $propertyGroup = $xml.SelectSingleNode("//PropertyGroup")
    if ($null -eq $propertyGroup) {
        $propertyGroup = $xml.CreateElement("PropertyGroup")
        $xml.Project.AppendChild($propertyGroup)
    }

    # Check if the property already exists
    $existingProperty = $propertyGroup.SelectSingleNode($PropertyName)

    if ($null -eq $existingProperty) {
        # If the property doesn't exist, create and add it
        $newProperty = $xml.CreateElement($PropertyName)
        $newProperty.InnerText = $PropertyValue
        $propertyGroup.AppendChild($newProperty)
    } else {
        # If the property exists, update its value
        $existingProperty.InnerText = $PropertyValue
    }

    $xml.Save($FileName)
}

Write-Host "Configuring project file for AWS Lambda tooling support"
Write-Host $args[0]

Add-AWSProjectTypeProperty -FileName $args[0]
Add-AWSProjectTypeProperty -FileName $args[0] -PropertyName "GenerateRuntimeConfigurationFiles" -PropertyValue "true"
Add-AWSProjectTypeProperty -FileName $args[0] -PropertyName "CopyLocalLockFileAssemblies" -PropertyValue "true"
Add-AWSProjectTypeProperty -FileName $args[0] -PropertyName "PublishReadyToRun" -PropertyValue "true"


