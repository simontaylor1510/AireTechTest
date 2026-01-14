var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL with persistent data volume
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("airetechtest-postgres-data")
    .WithPgAdmin();

var postgresdb = postgres.AddDatabase("postgresdb");

builder.AddProject<Projects.AireTechTest_Server>("server")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints()
    .WithUrls(context =>
    {
        if (context.GetEndpoint("https") is { } endpoint)
        {
            context.Urls.Clear();
            context.Urls.Add(new() { Url = $"{endpoint.Url}/scalar/v1", DisplayText = "Test API" });
        }
    });

builder.Build().Run();