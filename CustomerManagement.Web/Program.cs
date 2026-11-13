
using CustomerManagement.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Customer API HttpClient
builder.Services.AddHttpClient("CustomerAPI", client =>
{
    client.BaseAddress = new Uri("https+http://customermanagement-api");
});

// Aspire Service Defaults
builder.AddServiceDefaults();

// Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
