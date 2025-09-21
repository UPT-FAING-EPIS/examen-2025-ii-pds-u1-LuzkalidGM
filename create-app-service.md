# Crear App Service con Azure CLI

Si quieres crear un App Service nuevo con el nombre correcto:

```bash
# Login a Azure
az login

# Crear resource group (si no existe)
az group create --name projectmanagement-rg --location "Brazil South"

# Crear App Service Plan
az appservice plan create \
  --name projectmanagement-plan \
  --resource-group projectmanagement-rg \
  --sku F1 \
  --is-linux

# Crear App Service
az webapp create \
  --name projectmanagement-api \
  --resource-group projectmanagement-rg \
  --plan projectmanagement-plan \
  --runtime "DOTNETCORE:8.0"

# Descargar publish profile
az webapp deployment list-publishing-profiles \
  --name projectmanagement-api \
  --resource-group projectmanagement-rg \
  --xml
```

Copia la salida XML completa y úsala como AZURE_WEBAPP_PUBLISH_PROFILE.