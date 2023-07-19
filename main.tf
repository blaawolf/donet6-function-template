variable "DataDogApiKey" {}
variable "DataDogAgentUri" {}
variable "DataDogStatsDHost" {}
variable "DataDogLogUri" {}
variable "AppName" {}

terraform {
  required_providers {
     azurerm = {
       source  = "hashicorp/azurerm"
       version = "~> 3.32.0"
     }
  }
  backend "azurerm" {
    resource_group_name  = "rg-tamas-function-app-test"
    storage_account_name = "tfhackathon"
    container_name       = "tfstate"
    key                  = "{ApName}.tfstate" # set this to app name, variables cannot be used here
  }
}

provider "azurerm" {
  features {}
  skip_provider_registration = true
}

locals {
  resource_group_name     = "rg-tamas-function-app-test"
  resource_group_location = "North Europe"
  app_service_plan_name   = "${AppName}-function-hackathon-plan"
  app_service_name        = "${AppName}-function-hackathon"
  app_settings            = {
    "DataDogApiKey"           = var.DataDogApiKey
    "DataDogAgentUri"         = var.DataDogAgentUri
    "DataDogStatsDHost"       = var.DataDogStatsDHost
    "DataDogLogUri"           = var.DataDogLogUri
    "FUNCTIONS_WORKER_RUNTIME" = "dotnet-isolated"
  }
}

resource "azurerm_storage_account" "storageAccount" {
  name                     = "${AppName}storagehackathon"
  resource_group_name      = local.resource_group_name
  location                 = local.resource_group_location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

# Create Azure App Service Plan using Consumption pricing
resource azurerm_service_plan "primary" {
  name                = local.app_service_plan_name
  location            = local.resource_group_location
  resource_group_name = local.resource_group_name
  os_type             = "Linux"
  sku_name            = "Y1"
}

resource "azurerm_linux_function_app" "functionApp" {
  name                       = local.app_service_name
  resource_group_name        = local.resource_group_name
  location                   = local.resource_group_location
  service_plan_id            = azurerm_service_plan.primary.id
  storage_account_name       = azurerm_storage_account.storageAccount.name
  storage_account_access_key = azurerm_storage_account.storageAccount.primary_access_key
  https_only                 = true
  app_settings               = local.app_settings

  site_config {
    application_stack {
      dotnet_version = "6.0"
    }
  }
}