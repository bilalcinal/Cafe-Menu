using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Services;
using CafeMenu.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeMenu.Web.Controllers;

[Authorize(AuthenticationSchemes = "CustomerCookie")]
public class CustomerController : Controller
{
    private readonly CustomerMenuService _customerMenuService;
    private readonly ITenantRepository _tenantRepository;
    private readonly ITenantResolver _tenantResolver;
    private readonly IMenuPdfService _menuPdfService;

    public CustomerController(
        CustomerMenuService customerMenuService,
        ITenantRepository tenantRepository,
        ITenantResolver tenantResolver,
        IMenuPdfService menuPdfService)
    {
        _customerMenuService = customerMenuService;
        _tenantRepository = tenantRepository;
        _tenantResolver = tenantResolver;
        _menuPdfService = menuPdfService;
    }

    [RequirePermission("Customer.Menu.View")]
    public async Task<IActionResult> Index(int? categoryId, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);

        if (tenant == null)
        {
            return NotFound();
        }

        var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

        ViewBag.TenantName = tenant.Name;
        ViewBag.TenantCode = tenant.Code;
        ViewBag.UserName = userName;

        var viewModel = await _customerMenuService.GetMenuAsync(categoryId, cancellationToken);
        return View(viewModel);
    }

    [RequirePermission("Customer.Menu.View")]
    public async Task<IActionResult> Menu(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);

        if (tenant == null)
        {
            return NotFound();
        }

        var userName = User.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

        ViewBag.TenantName = tenant.Name;
        ViewBag.TenantCode = tenant.Code;
        ViewBag.UserName = userName;

        var viewModel = await _customerMenuService.GetCafeMenuAsync(cancellationToken);
        viewModel.TenantName = tenant.Name;

        return View(viewModel);
    }

    [RequirePermission("Customer.Menu.View")]
    public async Task<IActionResult> DownloadPdf(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);

        if (tenant == null)
        {
            return NotFound();
        }

        var menuData = await _customerMenuService.GetCafeMenuAsync(cancellationToken);
        menuData.TenantName = tenant.Name;

        var pdfBytes = await _menuPdfService.GenerateMenuPdfAsync(menuData, cancellationToken);

        var safeTenantName = System.Text.RegularExpressions.Regex.Replace(tenant.Name ?? "Menu", @"[^\w\s-]", "");
        safeTenantName = System.Text.RegularExpressions.Regex.Replace(safeTenantName, @"\s+", "_");
        if (string.IsNullOrWhiteSpace(safeTenantName))
            safeTenantName = "Menu";
        
        var fileName = $"{safeTenantName}_Menu_{DateTime.Now:yyyyMMdd}.pdf";
        var encodedFileName = Uri.EscapeDataString(fileName);
        
        Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"; filename*=UTF-8''{encodedFileName}");
        return File(pdfBytes, "application/pdf", fileName);
    }
}

