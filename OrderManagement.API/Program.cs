
using Microsoft.EntityFrameworkCore;
using OrderManagement.API.Data;
using OrderManagement.API.Messaging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Customer API communication
builder.Services.AddHttpClient("CustomerService", client =>
{
    client.BaseAddress = new Uri("https+http://customermanagement-api");
});

builder.Services.AddHttpClient("CustomerApi", client =>
{
    client.BaseAddress = new Uri("https+http://customermanagement-api");
});

// Aspire service defaults
builder.AddServiceDefaults();

// Order database connection
var orderConnectionString =
    builder.Configuration.GetConnectionString("OrderConnection");

if (string.IsNullOrWhiteSpace(orderConnectionString))
{
    throw new InvalidOperationException(
        "OrderConnection connection string is missing.");
}

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(orderConnectionString));

// Controllers and API documentation
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// RabbitMQ Consumer Registration
builder.Services.AddHostedService<CustomerCreatedConsumer>();

var app = builder.Build();

// Customer API test
app.MapGet("/api/orders/customer-test", async (
    IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("CustomerApi");

    var response = await client.GetAsync("/api/v1/customer/");

    var result = await response.Content.ReadAsStringAsync();

    return Results.Content(
        result,
        "application/json",
        statusCode: (int)response.StatusCode);
});

// Retry test
app.MapGet("/api/orders/retry-test", async (
    IHttpClientFactory httpClientFactory,
    ILogger<Program> logger) =>
{
    var client = httpClientFactory.CreateClient("CustomerApi");

    logger.LogInformation("Calling Customer API failure endpoint");

    var response = await client.GetAsync("/api/test-failure");

    logger.LogInformation(
        "Customer API returned: {StatusCode}",
        (int)response.StatusCode);

    return Results.StatusCode((int)response.StatusCode);
});

// Timeout test
app.MapGet("/api/orders/timeout-test", async (
    IHttpClientFactory httpClientFactory,
    ILogger<Program> logger) =>
{
    var client = httpClientFactory.CreateClient("CustomerApi");

    logger.LogInformation("Calling delayed Customer API");

    try
    {
        var response = await client.GetAsync("/api/test-delay");

        return Results.StatusCode((int)response.StatusCode);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Customer API request timed out");

        return Results.Problem(
            title: "Request timed out",
            statusCode: 504);
    }
});

// Aspire health checks
app.MapDefaultEndpoints();

// OpenAPI JSON
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Order CRUD controllers
try
{
    app.MapControllers();
}
catch (ReflectionTypeLoadException ex)
{
    foreach (var loaderException in ex.LoaderExceptions)
    {
        Console.WriteLine(loaderException?.ToString());
    }

    throw;
}

app.Run();
