# Events Cleanup - .NET Aspire

## 1. Run with .NET Aspire (Local Development)

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 22+](https://nodejs.org/)
- Access to a PostgreSQL database

### Steps

1. **Configure your FeatBit PostgreSQL connection** in `appsettings.Development.json`:

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

4. The **Aspire Dashboard** opens automatically with links to all services.

---

## 2. Run in Local Docker

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

### Steps

1. **Build and run with Docker Compose** (from solution root):

   ```bash
   # Build the API image
   docker build -t eventscleanupapi -f EventsCleanupApi/Dockerfile .

   # Build the webapp image
   docker build -t webapp -f webapp/Dockerfile .

   # Run with docker-compose (create docker-compose.yml if needed)
   docker-compose up
   ```

   Or use **Aspire manifest** to generate Docker artifacts:

   ```bash
   cd AppHost
   dotnet run -- --publisher manifest --output-path ../manifest.json
   ```

---

## 3. Deploy to Azure

### Prerequisites

- [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd)
- Azure subscription

### Steps

1. **Login to Azure**:

   ```bash
   azd auth login
   ```

2. **Initialize** (first time only, from solution root):

   ```bash
   azd init
   ```

   - Select "Use code in the current directory"
   - Confirm the AppHost project
   - Enter an environment name (e.g., `dev`)

3. **Deploy to Azure Container Apps**:

   ```bash
   azd up
   ```

   This will:
   - Provision Azure Container Apps environment
   - Create Azure Container Registry
   - Build and push container images
   - Deploy all services

4. **View deployed URLs** in the CLI output.

5. **Clean up** when done:

   ```bash
   azd down
   ```

