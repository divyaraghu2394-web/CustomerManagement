
var builder = WebApplication.CreateBuilder(args);

// Aspire service defaults
builder.AddServiceDefaults();

// Configure YARP Reverse Proxy
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddServiceDiscoveryDestinationResolver();

var app = builder.Build();

// Aspire health endpoints
app.MapDefaultEndpoints();

// Enable YARP
app.MapReverseProxy();

app.Run();
