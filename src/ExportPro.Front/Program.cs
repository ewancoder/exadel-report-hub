using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using ExportPro.Front;
using ExportPro.Front.Services;
using ExportPro.Front.Helper;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();


builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:1200") 
});

builder.Services.AddScoped<ApiHelper>();

builder.Services.AddScoped<UserStateService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
var host = builder.Build();
var userState = host.Services.GetRequiredService<UserStateService>();
await userState.InitializeAsync();

await builder.Build().RunAsync();
