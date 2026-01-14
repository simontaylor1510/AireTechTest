var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL with persistent data volume
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("airetechtest-postgres-data")
    .WithPgAdmin();

var postgresdb = postgres.AddDatabase("postgresdb");

var server = builder.AddProject<Projects.AireTechTest_Server>("server")
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();

builder.Build().Run();