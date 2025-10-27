using 'infrastructure.bicep'

param name = '[TODO-Add-LogicApp-Name]'
param location = 'northeurope'
param hostingPlanResourceId = '/subscriptions/[TODO-Add-Tenant-ID]/resourceGroups/Platform_LogicApp_Standard/providers/Microsoft.Web/serverfarms/ms-asp-platform-logicapps-dev'
param storageAccountName = '[TODO-Add-Storage-Account-Name]'
param logAnalyticsWorkspaceName = '[TODO-Add-LogicApp-Name]-logs'
param appInsightsWorkspaceName = '[TODO-Add-LogicApp-Name]-appinsights'
param keyVaultName = '[TODO-Add-LogicApp-Name]'
param keyVaultResourceGroup = '[TODO-Add-Resource-Group-Name]'




