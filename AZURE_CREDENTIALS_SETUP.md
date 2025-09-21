# AZURE_CREDENTIALS - Service Principal JSON

Como ya tienes el Service Principal creado, necesitas crear el JSON con esta estructura:

```json
{
  "clientId": "TU_APPLICATION_ID",
  "clientSecret": "TU_CLIENT_SECRET", 
  "subscriptionId": "TU_SUBSCRIPTION_ID",
  "tenantId": "TU_TENANT_ID"
}
```

## Cómo obtener los valores:

### 1. **clientId (Application ID)**
- Ve a Azure Portal → Azure Active Directory → App registrations
- Busca tu Service Principal
- Copia el **Application (client) ID**

### 2. **clientSecret** 
- En tu Service Principal → Certificates & secrets
- Si no tienes uno, crea un **New client secret**
- Copia el **Value** (no el Secret ID)

### 3. **subscriptionId**
- Azure Portal → Subscriptions
- Copia tu **Subscription ID**

### 4. **tenantId**
- En tu Service Principal → Overview
- Copia el **Directory (tenant) ID**

## Agregar en GitHub:

1. Ve a tu repositorio en GitHub
2. Settings → Secrets and variables → Actions
3. New repository secret
4. Name: `AZURE_CREDENTIALS`
5. Value: Pega el JSON completo (sin espacios extra)

## Ejemplo del JSON final:
```json
{
  "clientId": "12345678-1234-1234-1234-123456789012",
  "clientSecret": "tu_secret_super_secreto_aqui", 
  "subscriptionId": "87654321-4321-4321-4321-210987654321",
  "tenantId": "abcdefgh-abcd-abcd-abcd-abcdefghijkl"
}
```

⚠️ **IMPORTANTE**: 
- NO agregues espacios o saltos de línea extra
- Asegúrate de que el JSON sea válido
- El clientSecret es muy largo y solo se muestra una vez al crearlo