function DeployWebApp
{
<#
  .SYNOPSIS
  Deploys a template to Azure

  .DESCRIPTION
  Deploys an Azure Resource Manager template

  .PARAMETER subscriptionId
  The subscription id where the template will be deployed.

  .PARAMETER resourceGroupName
  The resource group where the template will be deployed. Can be the name of an existing or a new resource group.

  .PARAMETER resourceGroupLocation
  Optional, a resource group location. If specified, will try to create a new resource group in this location. If not specified, assumes resource group is existing.

  .PARAMETER deploymentName
  The deployment name.

  .PARAMETER templateFilePath
  Optional, path to the template file. Defaults to template.json.

  .PARAMETER parametersFilePath
  Optional, path to the parameters file. Defaults to parameters.json. If file is not found, will prompt for parameter values based on template.

  .PARAMETER $existingSite
  To specify the existing website or new website.
#>

param(
  [Parameter(Mandatory=$True)]
  [string]
  $subscriptionId,

  [Parameter(Mandatory=$True)]
  [string]
  $resourceGroupName,

  [string]
  $resourceGroupLocation,

  [Parameter(Mandatory=$True)]
  [string]
  $siteName,

  [string]
  $templateUri = "https://raw.githubusercontent.com/VeritasOS/arcus/master/Template.json",

  [Parameter(Mandatory=$True)]
  [string]
  $hostingPlanName,

  [Parameter(Mandatory=$True)]
  [string]
  $existingSite
   
)

#******************************************************************************
# Script body
# Execution begins here 
#******************************************************************************
$ErrorActionPreference = "Stop"
$Value = "Yes"

# sign in
Write-Host "Logging in...";
Add-AzureAccount;

# select subscription
Write-Host "Selecting subscription '$subscriptionId'";
Set-AzureRmContext -SubscriptionID $subscriptionId;

#Create or check for existing resource group
$resourceGroup = Get-AzureRmResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue
if(!$resourceGroup)
{
  Write-Host "Resource group '$resourceGroupName' does not exist. To create a new resource group, please enter a location.";
  if(!$resourceGroupLocation) {
    $resourceGroupLocation = Read-Host "resourceGroupLocation";
  }
  Write-Host "Creating resource group '$resourceGroupName' in location '$resourceGroupLocation'";
  New-AzureRmResourceGroup -Name $resourceGroupName -Location $resourceGroupLocation
}
else{
  Write-Host "Using existing resource group '$resourceGroupName'";
}



# Start the deployment
Write-Host "Starting deployment...";

    If($existingSite -eq $Value)
    {
    $site=Test-AzureName -Website $siteName -ErrorAction SilentlyContinue
     If($site)
        {
        # Remove existing webapp;
        Remove-AzureRmWebApp -ResourceGroupName $resourceGroupName -Name $siteName;
        Remove-AzureRmAppServicePlan -Name $hostingPlanName -ResourceGroupName $resourceGroupName -Force;
        }

    # Deploy new webapp;
    New-AzureRmResourceGroupDeployment -TemplateUri $templateUri -siteName $siteName -hostingPlanName $hostingPlanName -ResourceGroupName $resourceGroupName -Verbose;

    }

    Else
    {
    $site=Test-AzureName -Website $siteName
    while($site)
        {
        Write-Host "Site name '$siteName' already in use. Provide some other name.";
        $siteName=Read-Host "SiteName";
        $site=Test-AzureName -Website $siteName
        }

    # Start the deployment
    Write-Host "Starting deployment...";
    New-AzureRmResourceGroupDeployment -TemplateUri $templateUri -siteName $siteName -hostingPlanName $hostingPlanName -ResourceGroupName $resourceGroupName -Verbose
    }

}
 