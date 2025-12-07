# Events Cleanup - .NET Aspire

## 1. Run with .NET Aspire (Local Development)

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 22+](https://nodejs.org/)
- Access to a PostgreSQL database

### Steps

1. **Configure your FeatBit PostgreSQL connection** in `appsettings.Development.json` or `appsettings.json` in the `AppHost` folder:

   ```json
   {
     "ConnectionStrings": {
       "eventsdb": "Host=localhost;Database=featbit;Username=postgres;Password=postgres"
     }
   }
   ```

2. **Install webapp dependencies**:

   ```bash
   cd ../webapp
   npm install
   cd ../AppHost
   ```

3. **Run the AppHost**:

   ```bash
   dotnet run
   ```

Or use Visual Studio to run the `AppHost` project.

4. The **Aspire Dashboard** opens automatically with links to all services.

---

## 2. Deploy to Azure Container Apps

The project is configured to deploy to Azure Container Apps using the **Aspire CLI** with Azure CLI (`az`) authentication. The `AppHost` includes the `Aspire.Hosting.Azure.AppContainers` package which provides Azure infrastructure configuration.

### Prerequisites

- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
- [Aspire CLI](https://learn.microsoft.com/dotnet/aspire/cli/install) - Install with: `dotnet tool install -g Aspire.Cli`
- [Docker Desktop](https://www.docker.com/products/docker-desktop) - For building container images
- Azure subscription with appropriate permissions

### Steps

1. **Login to Azure CLI**:

   ```powershell
   az login
   ```

2. **Enable the deploy command** (one-time setup):

   ```powershell
   aspire config set features.deployCommandEnabled true
   ```

3. **Deploy to Azure Container Apps** (from solution root or AppHost directory):

   ```powershell
   aspire deploy
   ```

   The command will prompt you for:
   - Azure subscription
   - Azure location (region)
   - Resource group name

   This will automatically:
   - Provision Azure Container Apps environment
   - Create Azure Container Registry
   - Provision Log Analytics workspace for monitoring
   - Build and push container images
   - Deploy all services (API and webapp)

4. **View deployed URLs** in the CLI output.

5. **Monitor with Azure CLI**:

   ```powershell
   # List deployed container apps
   az containerapp list --resource-group <your-resource-group>

   # Get app endpoint
   az containerapp show --name eventscleanupapi --resource-group <your-resource-group> --query properties.configuration.ingress.fqdn
   ```

6. **Clean up** when done:

   ```powershell
   az group delete --name <your-resource-group>
   ```

---

## 3. Post-Deployment Configuration

After deploying with `aspire deploy`, you need to configure the PostgreSQL connection string and other settings.

### 3.1 Set PostgreSQL Connection String

The API requires a PostgreSQL connection string. Set it as a secret in Azure Container Apps:

```powershell
# Set the PostgreSQL connection string secret
az containerapp secret set `
  --name eventscleanupapi `
  --resource-group <your-resource-group> `
  --secrets 'connectionstrings--postgresql=Host=<host>;Port=5432;Username=<user>;Password=<password>;Database=<database>'

# Restart the app to apply changes
$revision = az containerapp revision list --name eventscleanupapi --resource-group <your-resource-group> --query "[0].name" -o tsv
az containerapp revision restart --name eventscleanupapi --resource-group <your-resource-group> --revision $revision
```

**Example** with actual values:
```powershell
az containerapp secret set `
  --name eventscleanupapi `
  --resource-group featbit-experimentals `
  --secrets 'connectionstrings--postgresql=Host=myserver.postgres.database.azure.com;Port=5432;Username=myuser;Password=MyP@ssword;Database=featbit'
```

### 3.2 Set Webapp API Backend URL

The webapp needs to know where to proxy API requests. Set the `API_BACKEND_URL` environment variable:

```powershell
# Update webapp with the API backend URL
az containerapp update `
  --name webapp `
  --resource-group <your-resource-group> `
  --set-env-vars "API_BACKEND_URL=https://eventscleanupapi.<your-aca-domain>.azurecontainerapps.io"
```

**Example**:
```powershell
az containerapp update `
  --name webapp `
  --resource-group featbit-experimentals `
  --set-env-vars "API_BACKEND_URL=https://eventscleanupapi.lemonmoss-cec5ddc5.westus2.azurecontainerapps.io"
```

### 3.3 Useful Azure CLI Commands

```powershell
# List all container apps in the resource group
az containerapp list --resource-group <your-resource-group> --output table

# Check environment variables of a container app
az containerapp show --name eventscleanupapi --resource-group <your-resource-group> --query "properties.template.containers[0].env" --output table

# View container app logs
az containerapp logs show --name eventscleanupapi --resource-group <your-resource-group> --type console --tail 50

# Check revision status
az containerapp revision list --name webapp --resource-group <your-resource-group> --query "[].{name:name, runningState:properties.runningState}" --output table

# Get the FQDN (URL) of a container app
az containerapp show --name webapp --resource-group <your-resource-group> --query "properties.configuration.ingress.fqdn" --output tsv
```

### 3.4 Redeploying After Code Changes

If you need to rebuild and redeploy after making code changes:

```powershell
# Option 1: Full redeploy with Aspire CLI
cd AppHost
aspire deploy --resource-group <your-resource-group>

# Option 2: Manual image update (faster for small changes)
# Build and push new image
cd webapp
docker build -t <acr-name>.azurecr.io/webapp:v2 .
az acr login --name <acr-name>
docker push <acr-name>.azurecr.io/webapp:v2

# Update container app with new image
az containerapp update --name webapp --resource-group <your-resource-group> --image <acr-name>.azurecr.io/webapp:v2
```

---

## 4. Configuration Notes

- The PostgreSQL connection string must be configured in Azure. You can use Azure Database for PostgreSQL or provide a connection string to an existing database.
- Set the `ConnectionStrings__PostgreSQL` environment variable in Azure Container Apps settings.
- External HTTP endpoints are enabled for both the API and webapp services.
- The webapp uses nginx to proxy `/api/*` requests to the backend API.
