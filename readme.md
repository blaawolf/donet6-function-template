## What does this template do?
This template includes:
* a simple .NET6 (C#) Azure Function project
* pre-installed Flipdish SDK
* Github Workflow for deployment
* Terraform template for creating Azure resources in a resource group that is shared between all apps that use this template (dev subscription).

## How to use this template? 
1. Press the 'Use this template' button on this repository and follow the instructions.
2. Replace '{AppName}' with your app name (lower case, no spaces)
   * in [.github/workflows/azure-functions-app-dotnet.yml](https://github.com/blaawolf/dotnet6-function-template/blob/7041e691de40669f46bfa1413fd9247b0aff5adf/.github/workflows/azure-functions-app-dotnet.yml#L8) file
   * in [main.tf](https://github.com/flipdishbytes/dotnet6-function-template/blob/7041e691de40669f46bfa1413fd9247b0aff5adf/main.tf#L18) file
3. #ask-delivery-enablement to set the necessary secrets in your new github repository

After successful deployment, you can access your function app at [{AppName}-function-hackathon.azurewebsites.net](https://github.com/flipdishbytes/dotnet6-function-template/blob/7041e691de40669f46bfa1413fd9247b0aff5adf/main.tf#L18)

[How to use Flipdish SDK?](https://github.com/flipdishbytes/api-client-csharp-netcore)
[How to create a Flipdish App Store app?](https://developers.flipdish.com/docs/how-to-create-a-flipdish-app-store-app)