using Blazored.LocalStorage;
using ExportPro.Front;
using ExportPro.Front.Helper;
using ExportPro.Front.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Attach root components
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// ? Local storage (for tokens etc.)
builder.Services.AddBlazoredLocalStorage();

// ? Authorization support (for [Authorize], etc.)
builder.Services.AddAuthorizationCore();

// ? Register HttpClient to talk to backend API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7067"), // ?? your API base URL
});

// ? ApiHelper that uses the above HttpClient
builder.Services.AddScoped<ApiHelper>();

// ? Auth state provider (optional, if you're tracking login)
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

// ?? Add other services as needed, e.g., AuthService, TokenService, etc.

await builder.Build().RunAsync();
