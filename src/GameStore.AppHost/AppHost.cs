var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume("gamestore-postgres-data");
var database = postgres.AddDatabase("gamestore");

builder.AddProject<Projects.GameStore_Api>("api")
    .WithReference(database)
    .WaitFor(database)
    .WithEndpoint("http", endpoint => endpoint.Port = 3000)
    .WithEndpoint("https", endpoint => endpoint.Port = 7000)
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health");

builder.Build().Run();
