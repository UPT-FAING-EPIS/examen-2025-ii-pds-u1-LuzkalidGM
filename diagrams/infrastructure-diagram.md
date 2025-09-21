# Diagrama de Infraestructura Azure

Este diagrama muestra la arquitectura de la infraestructura desplegada en Azure usando Terraform.

## Diagrama Terramaid
```mermaid
flowchart TD
    subgraph Terraform
        azurerm_mssql_database_sqldb["azurerm_mssql_database.sqldb"]
        azurerm_mssql_firewall_rule_sqlaccessrule["azurerm_mssql_firewall_rule.sqlaccessrule"]
        azurerm_mssql_server_sqlsrv["azurerm_mssql_server.sqlsrv"]
        azurerm_resource_group_rg["azurerm_resource_group.rg"]
    end
    azurerm_mssql_database_sqldb --> azurerm_mssql_server_sqlsrv
    azurerm_mssql_firewall_rule_sqlaccessrule --> azurerm_mssql_server_sqlsrv
    azurerm_mssql_server_sqlsrv --> azurerm_resource_group_rg
```


## Diagrama de Arquitectura

```mermaid
graph TB
    subgraph "Azure Subscription"
        subgraph "Resource Group: upt-arg-988"
            subgraph "Brazil South Region"
                SQL[("Azure SQL Server<br/>upt-dbs-988")]
                DB[("SQL Database<br/>shorten")]
                FW["Firewall Rule<br/>PublicAccess"]
                
                SQL --> DB
                SQL --> FW
            end
        end
    end
    
    subgraph "GitHub Actions"
        WF["Infrastructure<br/>Workflow"]
        DATA["Data Loading<br/>Workflow (Separado)"]
    end
    
    WF --> FW
    DATA -.-> FW
    FW --> SQL
    
    style SQL fill:#e1f5fe
    style DB fill:#f3e5f5
    style FW fill:#fff3e0
    style WF fill:#e8f5e8
    style DATA fill:#fff9c4,stroke-dasharray: 5 5
```

## Componentes Desplegados

### Resource Group: `upt-arg-988`
- **Región:** Brazil South
- **Propósito:** Contenedor de recursos

### SQL Server: `upt-dbs-988`
- **Versión:** 12.0
- **Autenticación:** SQL Server
- **Firewall:** Acceso público (0.0.0.0-255.255.255.255)

### Database: `shorten`
- **SKU:** Basic
- **Propósito:** Almacenar datos de empleo/PIB globales

## Estado de la Infraestructura

✅ **Infraestructura desplegada exitosamente**

### Próximos Pasos
1. ⏳ Ejecutar workflow de carga de datos por separado
2. � Consultar datos una vez cargados

---
*Generado automáticamente el $(date)*
