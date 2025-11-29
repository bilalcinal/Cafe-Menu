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

    public CustomerController(
        CustomerMenuService customerMenuService,
        ITenantRepository tenantRepository,
        ITenantResolver tenantResolver)
    {
        _customerMenuService = customerMenuService;
        _tenantRepository = tenantRepository;
        _tenantResolver = tenantResolver;
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
}

