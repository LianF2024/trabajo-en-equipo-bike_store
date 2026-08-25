using BikeStore.Web.Models;
using BikeStore.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace BikeStore.Web.Controllers;

public sealed class HomeController(IBikeStoreApiClient api) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var bicyclesTask = api.GetBicyclesAsync(cancellationToken: cancellationToken);
            var lowStockTask = api.GetBicyclesAsync(lowStock: true, cancellationToken: cancellationToken);
            var customersTask = api.GetCustomersAsync(cancellationToken: cancellationToken);
            var salesTask = api.GetSalesAsync(cancellationToken: cancellationToken);
            await Task.WhenAll(bicyclesTask, lowStockTask, customersTask, salesTask);
            var bicycles = await bicyclesTask;
            var sales = await salesTask;
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var todaySales = sales.Where(x => x.Date.Date == today).ToList();
            var monthSales = sales.Where(x => x.Date.Date >= monthStart && x.Date.Date <= today).ToList();

            return View(new DashboardVm
            {
                Bicycles = bicycles.Count,
                LowStock = (await lowStockTask).Count,
                Customers = (await customersTask).Count,
                TodaySales = todaySales.Count,
                TodayRevenue = todaySales.Sum(x => x.Total),
                TotalStock = bicycles.Sum(x => x.Stock),
                OutOfStock = bicycles.Count(x => x.Stock == 0),
                MonthSales = monthSales.Count,
                MonthRevenue = monthSales.Sum(x => x.Total),
                RecentSales = sales.OrderByDescending(x => x.Date).Take(5).ToList()
            });
        }
        catch (ApiException exception)
        {
            ViewBag.ApiError = exception.Message;
            return View(new DashboardVm());
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(string? message)
    {
        ViewBag.ErrorMessage = message;
        return View();
    }
}
