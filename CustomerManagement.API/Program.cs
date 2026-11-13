
using Asp.Versioning;
using CustomerManagement.Core.Interfaces;
using CustomerManagement.Core.Mapping;
using CustomerManagement.Core.Services;
using CustomerManagement.Core.Validators;
using CustomerManagement.Infrastructure.Data;
using CustomerManagement.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Rate Limiting Configuration
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("CustomerPolicy", limiter =>
    {
        limiter.PermitLimit = 5;
        limiter.Window = TimeSpan.FromSeconds(10);
        limiter.QueueLimit = 0;
        limiter.AutoReplenishment = true;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.AddServiceDefaults();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30
    )
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddMvc();

// OpenAPI configuration
builder.Services.AddOpenApi();

// Register AutoMapper
builder.Services.AddAutoMapper(
    cfg => { },
    typeof(CustomerMappingProfile)
);

builder.Services.AddValidatorsFromAssemblyContaining<CustomerCreateDtoValidator>();

// SQL Server Database Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();


var app = builder.Build();

app.UseRateLimiter();

var environmentName = Environment.GetEnvironmentVariable("APP_ENV_NAME");

Console.WriteLine($"Environment Variable: {environmentName}");

app.MapGet("/api/test-failure", () =>
{
    return Results.StatusCode(503);
});

app.MapGet("/api/test-delay", async () =>
{
    await Task.Delay(TimeSpan.FromSeconds(10));

    return Results.Ok("Customer API responded after 10 seconds");
});

Log.Information("Aspire test: Customer Management API started");

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
