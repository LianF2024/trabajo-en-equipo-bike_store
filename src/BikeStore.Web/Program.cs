using BikeStore.Web.Filters;
using BikeStore.Web.Services;
using System.Globalization;

var culture = CultureInfo.GetCultureInfo("es-EC");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews(options =>
    options.Filters.Add<ApiExceptionFilter>());
builder.Services.AddHttpClient<IBikeStoreApiClient, BikeStoreApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7101/");
    client.Timeout = TimeSpan.FromMinutes(5);
});

var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
