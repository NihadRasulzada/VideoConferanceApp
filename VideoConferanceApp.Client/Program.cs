using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using NetcodeHub.Packages.Extensions.LocalStorage;
using VideoConferanceApp.Client;
using VideoConferanceApp.Client.Extensions;
using VideoConferanceApp.Client.Services;
using VideoConferanceApp.Client.States;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddFluentUIComponents();
builder.Services.AddNetcodeHubLocalStorageService();

string clientName = builder.Configuration["HttpClient:Name"]! ??
                    throw new InvalidOperationException("Config Setup for HttpClient not found");

builder.Services.AddScoped<HttpDelegate>();

builder.Services.AddHttpClient(clientName, options =>
{
    string baseAddress = builder.Configuration["Server:BaseAddress"]! ??
                         throw new InvalidOperationException("Base Address not found");

    options.BaseAddress = new Uri($"{baseAddress}/api/");
}).AddHttpMessageHandler<HttpDelegate>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITwilioService, TwilioService>();
builder.Services.AddScoped<IMeetingService, MeetingService>();

builder.Services.AddScoped<IHttpExtension, HttpExtension>();

builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
    provider.GetRequiredService<CustomAuthStateProvider>());

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<NavState>();
await builder.Build().RunAsync();