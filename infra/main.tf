terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0.0"
    }
  }
  required_version = ">= 0.14.9"
}


variable "suscription_id" {
  type        = string
  description = "Azure subscription id"
  default     = "25a89e19-1e5d-454b-8146-1ec4d976f554"
}

variable "sqladmin_username" {
  type        = string
  description = "Administrator username for server"
  default     = "adminuser"
}

variable "sqladmin_password" {
  type        = string
  description = "Administrator password for server"
  default     = "Admin123456"
}

provider "azurerm" {
  features {}
  subscription_id = var.suscription_id
}

## Eliminado random_integer para usar nombres fijos

resource "azurerm_resource_group" "rg" {
  name     = "upt-arg-988"
  location = "brazilsouth"
}


resource "azurerm_mssql_server" "sqlsrv" {
  name                         = "upt-dbs-988"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = var.sqladmin_username
  administrator_login_password = var.sqladmin_password
}

resource "azurerm_mssql_firewall_rule" "sqlaccessrule" {
  name             = "PublicAccess"
  server_id        = azurerm_mssql_server.sqlsrv.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "255.255.255.255"
}

resource "azurerm_mssql_database" "sqldb" {
  name      = "shorten"
  server_id = azurerm_mssql_server.sqlsrv.id
  sku_name  = "Basic"
}