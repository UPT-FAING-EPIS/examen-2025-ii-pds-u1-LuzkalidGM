# Configuración de Azure para Deployment

## Opción 1: Service Principal (Para AZURE_CREDENTIALS)

### Paso 1: Crear Service Principal en Azure Portal

1. Ve a [Azure Portal](https://portal.azure.com)
2. Busca "Azure Active Directory" o "Microsoft Entra ID"
3. Ve a "App registrations" → "New registration"
4. Nombre: `github-deployment-sp`
5. Copia el **Application (client) ID**
6. Ve a "Certificates & secrets" → "New client secret"
7. Copia el **Client Secret Value** (solo se muestra una vez!)
8. Ve a "Overview" y copia el **Directory (tenant) ID**

### Paso 2: Obtener Subscription ID

1. En Azure Portal, busca "Subscriptions"
2. Copia tu **Subscription ID**

### Paso 3: Dar permisos al Service Principal

1. Ve a tu Resource Group o Subscription
2. Ve a "Access control (IAM)" → "Add role assignment"
3. Role: "Contributor"
4. Assign access to: "User, group, or service principal"
5. Busca el nombre de tu app registration: `github-deployment-sp`
6. Save

### Paso 4: Crear el JSON para AZURE_CREDENTIALS

Crea un JSON con esta estructura:

```json
{
  "clientId": "TU_APPLICATION_CLIENT_ID",
  "clientSecret": "TU_CLIENT_SECRET_VALUE", 
  "subscriptionId": "TU_SUBSCRIPTION_ID",
  "tenantId": "TU_DIRECTORY_TENANT_ID"
}
```

### Paso 5: Agregar Secret en GitHub

1. Ve a tu repositorio en GitHub
2. Settings → Secrets and variables → Actions
3. New repository secret
4. Name: `AZURE_CREDENTIALS`
5. Value: Pega el JSON completo

---

## Opción 2: Solo Publish Profile (Más Simple)

Si quieres evitar Service Principal, puedes usar solo el Publish Profile:

### Paso 1: Crear App Service en Azure

1. Ve a Azure Portal
2. Create a resource → Web App
3. Nombre: `projectmanagement-app` (debe coincidir con el workflow)
4. Runtime: .NET 8
5. Operating System: Windows o Linux
6. Región: Brazil South

### Paso 2: Descargar Publish Profile

1. Ve a tu App Service creado
2. Overview → "Get publish profile"
3. Se descarga un archivo `.publishsettings`
4. Abre el archivo y copia TODO el contenido XML

### Paso 3: Agregar Secret en GitHub

1. Ve a tu repositorio en GitHub
2. Settings → Secrets and variables → Actions  
3. New repository secret
4. Name: `AZURE_WEBAPP_PUBLISH_PROFILE`
5. Value: Pega todo el contenido XML del archivo .publishsettings

---

## Opción 3: Azure CLI (Para desarrolladores)

Si tienes Azure CLI instalado:

```bash
# Login
az login

# Crear service principal
az ad sp create-for-rbac --name "github-deployment" --role contributor --scopes /subscriptions/YOUR_SUBSCRIPTION_ID --sdk-auth

# Esto te dará el JSON completo para AZURE_CREDENTIALS
```

---

## Recomendación

**Para empezar rápido**: Usa la Opción 2 (solo Publish Profile)
**Para producción**: Usa la Opción 1 (Service Principal) 

El workflow actual está configurado para funcionar con ambas opciones.