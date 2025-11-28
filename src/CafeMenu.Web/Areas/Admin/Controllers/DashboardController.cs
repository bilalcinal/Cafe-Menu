using CafeMenu.Application.Services;
using CafeMenu.Application.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeMenu.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class DashboardController : Controller
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var viewModel = await _dashboardService.GetDashboardDataAsync(cancellationToken);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> GetRates(CancellationToken cancellationToken = default)
    {
        var viewModel = await _dashboardService.GetDashboardDataAsync(cancellationToken);
        return Json(viewModel.CurrencyRates);
    }
}

