using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
//using Patronus.API;
using Patronus.Client;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
//builder.Services.AddRefitClient<IPatronusApiClient>().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.github.com")); ;

await builder.Build().RunAsync();
