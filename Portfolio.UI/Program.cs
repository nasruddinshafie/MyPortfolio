using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Portfolio.UI;
using Portfolio.UI.Services;
using Portfolio.UI.Handlers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add Authentication services
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

// Register the AuthTokenHandler
builder.Services.AddScoped<AuthTokenHandler>();

// Configure HTTP client for API calls with base URL
// AuthService doesn't need the token handler (login/register are public endpoints)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7278/") });

// BioService and ProjectService need the auth token handler for protected endpoints
builder.Services.AddHttpClient<IBioService, BioService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7278/");
})
.AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<IProjectService, ProjectService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7278/");
})
.AddHttpMessageHandler<AuthTokenHandler>();

// ContactService - public endpoint (submit) but also has protected endpoints (admin view)
builder.Services.AddHttpClient<IContactService, ContactService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7278/");
});

await builder.Build().RunAsync();
