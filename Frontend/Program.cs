using Frontend;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

builder.Services.AddMudServices();
builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    //BaseAddress = new Uri(builder.Configuration["AppSettings:ApiBaseUrl"] 
    //?? throw new InvalidDataException("δ����API��ַ"))
});

await builder.Build().RunAsync();
