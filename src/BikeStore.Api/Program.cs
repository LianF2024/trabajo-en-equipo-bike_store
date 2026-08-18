using BikeStore.Api.Middleware;
using BikeStore.Application.Common;
using BikeStore.Application.Contracts;
using BikeStore.Application.Interfaces;
using BikeStore.Application.Services;
using BikeStore.Infrastructure.Data;
using BikeStore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var culture = CultureInfo.GetCultureInfo("es-EC");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "BikeStore API", Version = "v1", Description = "Servicios REST para categorías, bicicletas, clientes e inventario/ventas." });
});
var connectionString = builder.Configuration.GetConnectionString("BikeStore")
    ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'BikeStore'.");

builder.Services.AddDbContext<BikeStoreDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBicycleService, BicycleService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddSingleton(new BusinessOptions
{
    VatRate = builder.Configuration.GetValue<decimal>("Business:VatRate", 0.15m),
    LowStockThreshold = builder.Configuration.GetValue<int>("Business:LowStockThreshold", 5)
});
builder.Services.AddHealthChecks().AddDbContextCheck<BikeStoreDbContext>();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");
app.Run();

public partial class Program;
