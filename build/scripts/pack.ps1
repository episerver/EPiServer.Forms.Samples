Param([string]$versionSuffix = "",[string] $configuration = "Release")

# Set location to the Solution directory
(Get-Item $PSScriptRoot).Parent.FullName | Push-Location

[xml] $versionFile = Get-Content "..\build\props\dependencies.props"

$core_node = $versionFile.SelectSingleNode("Project/PropertyGroup/CmsCoreVersion")
$coreVersion = $core_node.InnerText

$forms_node = $versionFile.SelectSingleNode("Project/PropertyGroup/FormVersion")
$formsVersion = $forms_node.InnerText
$formsParts = $formsVersion.Split(".")
$formsMajor = [int]::Parse($formsParts[0]) + 1
$formsNextMajorVersion = ($formsMajor.ToString() + ".0.0") 

$formsNextMajorVersion

[xml] $versionFile = Get-Content "..\build\props\version.props"
$pVersion = $versionFile.SelectSingleNode("Project/PropertyGroup/VersionPrefix").InnerText + $versionSuffix

# Packaging public packages
dotnet pack --no-restore --no-build --output ../nupkgs -c $configuration /p:Version=$pVersion  /p:FormsVersion=$formsVersion /p:FormsNextMajorVersion=$formsNextMajorVersion /p:CoreVersion=$coreVersion ../src/EPiServer.Forms.Samples/EPiServer.Forms.Samples.csproj

Pop-Location