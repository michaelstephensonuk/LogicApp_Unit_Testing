
# Notes

1) Hosting Plan

I am assuming that the hosting plan has been deployed seperately to this solution as 1 plan per logic app is very expensive
The plan is likely to be part of some kind of landing zone approach seperate to the application







We can just run the script
```
# Deploying Logic App with Bicep
az deployment group create \
  --resource-group [TODO-Add-Resource-Group-Name] \
  --template-file ./logicapp.bicep \
  --parameters name=[TODO-Add-LogicApp-Name] \
               location=northeurope \
               hostingPlanName=ms-asp-platform-logicapps-dev \
               hostingPlanResourceGroup=Platform_LogicApp_Standard \
               hostingPlanId="/subscriptions/[TODO-Add-Tenant-ID]/resourceGroups/Platform_LogicApp_Standard/providers/Microsoft.Web/serverfarms/ms-asp-platform-logicapps-dev" \
               appInsightsName=[TODO-Add-LogicApp-Name] \
               appInsightsResourceGroup=[TODO-Add-Resource-Group-Name] \
               appInsightsId="/subscriptions/[TODO-Add-Tenant-ID]/resourceGroups/[TODO-Add-Resource-Group-Name]/providers/microsoft.insights/components/[TODO-Add-LogicApp-Name]" \
               storageAccountName=[TODO-Add-Storage-Account-Name]
```




