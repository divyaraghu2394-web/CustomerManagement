
var builder = DistributedApplication.CreateBuilder(args);

// SQL Server connection string
var sqlConnectionString = builder.AddConnectionString("DefaultConnection");

// Customer API
var customerApi = builder.AddProject<Projects.CustomerManagement_API>(
    "customermanagement-api")
    .WithReference(sqlConnectionString)
    .WithEnvironment("APP_ENV_NAME", "Aspire Development");

// Order API
builder.AddProject<Projects.OrderManagement_API>(
    "ordermanagement-api")
    .WithReference(customerApi);

// Blazor Web Application
builder.AddProject<Projects.CustomerManagement_Web>(
    "customermanagement-web");

builder.Build().Run();