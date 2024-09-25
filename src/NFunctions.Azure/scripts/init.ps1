function Add-AzureFunctionsProjectTypeProperty {
    [OutputType([System.Xml.XmlDocument])]
    param(
        [Parameter(Mandatory=$true)]
        [string]$FileName,
        
        [Parameter(Mandatory=$false)]
        [string]$PropertyName = "AzureFunctionsVersion",
        
        [Parameter(Mandatory=$false)]
        [string]$PropertyValue = "v4"
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

function Add-AzureFunctionsSdkReference {
    param (
        [Parameter(Mandatory=$true)]
        [string]$FileName,
        
        [Parameter(Mandatory=$false)]
        [string]$Version = "4.2.0"
    )

    # Load the XML content of the .csproj file
    $csproj = [xml](Get-Content $FileName)

    # Check if the PackageReference already exists
    $existingReference = $csproj.Project.ItemGroup.PackageReference | 
                         Where-Object { $_.Include -eq "Microsoft.NET.Sdk.Functions" }

    if (!$existingReference) {
        
        # Create a new PackageReference element
        $newReference = $csproj.CreateElement("PackageReference")
        $newReference.SetAttribute("Include", "Microsoft.NET.Sdk.Functions")
        $newReference.SetAttribute("Version", $Version)

        # Find or create an ItemGroup to add the PackageReference
        $itemGroup = $csproj.Project.ItemGroup | Where-Object { $_.PackageReference }
        if (-not $itemGroup) {
            $itemGroup = $csproj.CreateElement("ItemGroup")
            $csproj.Project.AppendChild($itemGroup)
        }

        # Add the new PackageReference to the ItemGroup
        $itemGroup.AppendChild($newReference)

        # Save the changes
        $csproj.Save($FileName)

        Write-Host "Added Microsoft.NET.Sdk.Functions reference with version $Version"
    }
}

Write-Host "Configuring project file for Azure Functions tooling support"
Write-Host $args[0]

Add-AzureFunctionsProjectTypeProperty -FileName $args[0]
Add-AzureFunctionsSdkReference -FileName $args[0]


