using Backend.Data;
using Backend.Repositories;
using Backend.Services;
using Backend.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<SupaBaseDbContext>(); 
builder.Services.AddScoped<SpeedrunRepository>();
builder.Services.AddScoped<ISpeedrunService, SpeedrunService>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
