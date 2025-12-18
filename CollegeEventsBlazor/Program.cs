using CollegeEventsBlazor;
using CollegeEventsBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBase = builder.Configuration["ApiBaseUrl"];
if (string.IsNullOrWhiteSpace(apiBase))
    throw new Exception("ApiBaseUrl missing in wwwroot/appsettings.json");

apiBase = apiBase.TrimEnd('/') + "/";

builder.Services.AddScoped<TokenStore>();
builder.Services.AddScoped<JwtHelper>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

builder.Services.AddScoped<ApiAuthMessageHandler>();


builder.Services.AddHttpClient("Api", client =>
{
    client.BaseAddress = new Uri(apiBase);
}).AddHttpMessageHandler<ApiAuthMessageHandler>();


builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<StudentsService>();

await builder.Build().RunAsync();
