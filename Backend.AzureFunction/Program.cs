using Backend.Data;
using Backend.Repositories;
using Backend.Services;
using Backend.Services.Interfaces;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();
builder.Services.AddDbContext<SupaBaseDbContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("SupaBase")
        ?? throw new InvalidDataException("δ����Supabase�������ַ���"));
});
// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();
//builder.Services.AddHttpClient(LeanCloudConstants.HttpClientName, httpClient =>
//{
//    httpClient.BaseAddress = new Uri($"{Environment.GetEnvironmentVariable("LEANCLOUD_REST_API_URL")
//        ?? throw new InvalidDataException("δ����LeanCloud��REST API��ַ")}{LeanCloudConstants.RequestPrefix}/");
//    httpClient.DefaultRequestHeaders.Add("X-LC-Id", Environment.GetEnvironmentVariable("LEANCLOUD_APP_ID")
//        ?? throw new InvalidDataException("δ����LeanCloud��AppID"));
//    httpClient.DefaultRequestHeaders.Add("X-LC-Key", Environment.GetEnvironmentVariable("LEANCLOUD_APP_KEY")
//        ?? throw new InvalidDataException("δ����LeanCloud��AppKey"));
//});
//builder.Services.AddSingleton(typeof(ILeanCloudRepository<>), typeof(LeanCloudRepository<>));
builder.Services.AddScoped<SpeedrunRepository>();
builder.Services.AddScoped<ISpeedrunService, SpeedrunService>();
builder.Build().Run();
