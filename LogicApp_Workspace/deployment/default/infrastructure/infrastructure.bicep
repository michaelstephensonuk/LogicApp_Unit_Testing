/* 
Overview
This Bicep file deploys a Logic App with a System Assigned Managed Identity, linked to an existing Application Insights resource and a hosting plan.
It also provisions a storage account for the Logic App's use.
*/

@description('Parameters for Logic App deployment')
param name string
param location string
param hostingPlanResourceId string
param storageAccountName string
param logAnalyticsWorkspaceName string
param appInsightsWorkspaceName string
param keyVaultName string
param keyVaultResourceGroup string


//This keyvault is in a different resource group so I am passing in a reference to it
resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' existing = {
  name: keyVaultName
  scope: resourceGroup(keyVaultResourceGroup)
}

// Log Analytics workspace
resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2025-02-01' = {
  name: logAnalyticsWorkspaceName
  location: location
  properties: {
    sku: {
      name: 'pergb2018'
    }
    retentionInDays: 30
    features: {
      enableLogAccessUsingOnlyResourcePermissions: true
    }
    workspaceCapping: {
      dailyQuotaGb: json('1')
    }
  }
}

// App Insights linked to Log Analytics workspace
resource applicationInsights 'microsoft.insights/components@2020-02-02' = {
  name: appInsightsWorkspaceName
  location: 'northeurope'
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Flow_Type: 'Redfield'
    Request_Source: 'IbizaAIExtension'
    RetentionInDays: 90
    WorkspaceResourceId: logAnalytics.id
    IngestionMode: 'LogAnalytics'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

// Creates a storage account for the Logic App
resource storageAccount 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {
    supportsHttpsTrafficOnly: true
    minimumTlsVersion: 'TLS1_2'
  }
}

// Deploys the Logic App with a System Assigned Managed Identity
resource logicApp 'Microsoft.Web/sites@2018-11-01' = {
  name: name
  location: location
  kind: 'functionapp,workflowapp'
  identity: {
    type: 'SystemAssigned'
  }
  tags: {
    'hidden-link: /app-insights-resource-id': applicationInsights.id
  }
  properties: {
    siteConfig: {
      appSettings: [
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'node'
        }
        {
          name: 'WEBSITE_NODE_DEFAULT_VERSION'
          value: '~14'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: applicationInsights.properties.ConnectionString
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${listKeys(storageAccount.id, '2019-06-01').keys[0].value};EndpointSuffix=core.windows.net'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${listKeys(storageAccount.id, '2019-06-01').keys[0].value};EndpointSuffix=core.windows.net'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: '${toLower(name)}97b0'
        }
        {
          name: 'AzureFunctionsJobHost__extensionBundle__id'
          value: 'Microsoft.Azure.Functions.ExtensionBundle.Workflows'
        }
        {
          name: 'AzureFunctionsJobHost__extensionBundle__version'
          value: '[1.*, 2.0.0)'
        }
        {
          name: 'APP_KIND'
          value: 'workflowApp'
        }
        {
          name: 'CustomTestSetting'
          value: 'test-1-2-3'
        }
        {
          name: 'Azure_Open_AI_GPT_EndPoint'          
          value: '@Microsoft.KeyVault(SecretUri=${keyVault.properties.vaultUri}secrets/Azure-Open-AI-EndPoint/)'
        }
        {
          name: 'Azure_Open_AI_GPT_Key'          
          value: '@Microsoft.KeyVault(SecretUri=${keyVault.properties.vaultUri}secrets/Azure-Open-AI-Key/)'
        }
        {
          name: 'agent_openAIEndpoint'          
          value: '@Microsoft.KeyVault(SecretUri=${keyVault.properties.vaultUri}secrets/Azure-Open-AI-BaseUrl/)'
        }
        {
          name: 'agent_openAIKey'          
          value: '@Microsoft.KeyVault(SecretUri=${keyVault.properties.vaultUri}secrets/Azure-Open-AI-Key/)'
        }
        {
          name: 'AzureCosmosDB_connectionString'          
          value: '@Microsoft.KeyVault(SecretUri=${keyVault.properties.vaultUri}secrets/AzureCosmosDB-ConnectionString/)'
        }

        {
          name: 'AzureBlob_connectionString'
          value: '@Microsoft.KeyVault(SecretUri=${keyVault.properties.vaultUri}secrets/AzureBlob-ConnectionString/)'
        }
        


      ]
      cors: {}
      use32BitWorkerProcess: true
      netFrameworkVersion: 'v8.0'
      ftpsState: 'FtpsOnly'
    }
    serverFarmId: hostingPlanResourceId
    clientAffinityEnabled: false
    httpsOnly: true
  }
}

output logicAppSystemAssignedIdentityTenantId string = subscription().tenantId
output logicAppSystemAssignedIdentityObjectId string = logicApp.identity.principalId
output logAnalyticsWorkspaceId string = logAnalytics.id
output logAnalyticsCustomerId string = logAnalytics.properties.customerId
