var builder = DistributedApplication.CreateBuilder(args);

// PostgreSQL connection string from AppHost appsettings.json / appsettings.Development.json
// This will be passed to EventsCleanupApi as ConnectionStrings__PostgreSQL environment variable
var postgres = builder.AddConnectionString("PostgreSQL");

// Add the Events Cleanup API with PostgreSQL connection string reference
var api = builder.AddProject<Projects.EventsCleanupApi>("eventscleanupapi")
    .WithReference(postgres)
    .WithExternalHttpEndpoints();

// Add the React webapp (Vite dev server)
var webapp = builder.AddViteApp("webapp", "../webapp")
    .WithReference(api)
    .WithEnvironment("VITE_API_URL", api.GetEndpoint("https"))
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.Build().Run();
