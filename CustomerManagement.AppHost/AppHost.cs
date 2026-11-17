
var builder = DistributedApplication.CreateBuilder(args);

// Customer database connection
var sqlConnectionString =
    builder.AddConnectionString("DefaultConnection");

// Order database connection
var orderConnection =
    builder.AddConnectionString("OrderConnection");

// Customer API
var customerApi =
    builder.AddProject<Projects.CustomerManagement_API>(
        "customermanagement-api")
    .WithReference(sqlConnectionString)
    .WithEnvironment("APP_ENV_NAME", "Aspire Development");

// Order API
var orderApi =
    builder.AddProject<Projects.OrderManagement_API>(
        "ordermanagement-api")
    .WithReference(customerApi)
    .WithReference(orderConnection);

// Blazor Web Application
builder.AddProject<Projects.CustomerManagement_Web>(
    "customermanagement-web")
    .WithReference(customerApi);

// API Gateway
builder.AddProject<Projects.CustomerManagement_Gateway>(
"customermanagement-gateway")
    .WithReference(customerApi)
    .WithReference(orderApi);

builder.Build().Run();
