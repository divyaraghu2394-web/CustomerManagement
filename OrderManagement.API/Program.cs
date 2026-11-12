var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("CustomerApi", client =>
{
    client.BaseAddress = new Uri("https+http://customermanagement-api");
});

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapGet("/api/orders/customer-test", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient("CustomerApi");

    var response = await client.GetAsync("/api/v1/customer/");

    var result = await response.Content.ReadAsStringAsync();

    return Results.Content(
        result,
        "application/json",
        statusCode: (int)response.StatusCode
    );
});

app.MapGet("/api/orders/retry-test", async (
    IHttpClientFactory httpClientFactory,
    ILogger<Program> logger) =>
{
    var client = httpClientFactory.CreateClient("CustomerApi");

    logger.LogInformation("Calling Customer API failure endpoint");

    var response = await client.GetAsync("/api/test-failure");

    logger.LogInformation("Customer API returned: {StatusCode}",
        (int)response.StatusCode);

    return Results.StatusCode((int)response.StatusCode);
});

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

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
