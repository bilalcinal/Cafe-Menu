using CafeMenu.Application.Services;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeMenu.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class DashboardController : Controller
{
    private readonly DashboardService _dashboardService;
    private readonly ICurrencyService _currencyService;

    public DashboardController(DashboardService dashboardService, ICurrencyService currencyService)
    {
        _dashboardService = dashboardService;
        _currencyService = currencyService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var viewModel = await _dashboardService.GetDashboardDataAsync(cancellationToken);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> CurrencyWidget(CancellationToken cancellationToken = default)
    {
        var rates = await _currencyService.GetLatestRatesAsync(cancellationToken);

        return Json(new
        {
            usd = rates.UsdRate,
            eur = rates.EurRate,
            @try = rates.TryRate,
            fetchedAt = DateTime.UtcNow
                .ToLocalTime()
                .ToString("dd.MM.yyyy HH:mm:ss")
        });
    }


    [HttpGet]
    public async Task<IActionResult> ProductCountWidget(CancellationToken cancellationToken = default)
    {
        var viewModel = await _dashboardService.GetDashboardDataAsync(cancellationToken);
        var productCounts = viewModel.ProductCountByCategory
            .OrderByDescending(x => x.Value)
            .Select(kvp => new { categoryName = kvp.Key, productCount = kvp.Value })
            .ToList();
        return Json(productCounts);
    }
}

