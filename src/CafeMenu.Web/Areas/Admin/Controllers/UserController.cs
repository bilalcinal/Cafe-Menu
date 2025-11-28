using CafeMenu.Application.Services;
using CafeMenu.Application.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeMenu.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class UserController : Controller
{
    private readonly UserManagementService _userManagementService;
    private readonly TenantService _tenantService;

    public UserController(UserManagementService userManagementService, TenantService tenantService)
    {
        _userManagementService = userManagementService;
        _tenantService = tenantService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var users = await _userManagementService.GetAllAsync(cancellationToken);
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
    {
        var tenants = await _tenantService.GetAllAsync(cancellationToken);
        var viewModel = new UserViewModel
        {
            AvailableTenants = tenants.Select(t => new CafeMenu.Application.Models.TenantDto
            {
                TenantId = t.TenantId,
                Name = t.Name,
                Code = t.Code,
                IsActive = t.IsActive,
                CreatedDate = t.CreatedDate
            }).ToList()
        };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserViewModel viewModel, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            var tenants = await _tenantService.GetAllAsync(cancellationToken);
            viewModel.AvailableTenants = tenants.Select(t => new CafeMenu.Application.Models.TenantDto
            {
                TenantId = t.TenantId,
                Name = t.Name,
                Code = t.Code,
                IsActive = t.IsActive,
                CreatedDate = t.CreatedDate
            }).ToList();
            return View(viewModel);
        }

        try
        {
            await _userManagementService.CreateAsync(viewModel, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var tenants = await _tenantService.GetAllAsync(cancellationToken);
            viewModel.AvailableTenants = tenants.Select(t => new CafeMenu.Application.Models.TenantDto
            {
                TenantId = t.TenantId,
                Name = t.Name,
                Code = t.Code,
                IsActive = t.IsActive,
                CreatedDate = t.CreatedDate
            }).ToList();
            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, int tenantId, CancellationToken cancellationToken = default)
    {
        var viewModel = await _userManagementService.GetByIdAsync(id, tenantId, cancellationToken);
        if (viewModel == null)
        {
            return NotFound();
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserViewModel viewModel, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            var tenants = await _tenantService.GetAllAsync(cancellationToken);
            viewModel.AvailableTenants = tenants.Select(t => new CafeMenu.Application.Models.TenantDto
            {
                TenantId = t.TenantId,
                Name = t.Name,
                Code = t.Code,
                IsActive = t.IsActive,
                CreatedDate = t.CreatedDate
            }).ToList();
            return View(viewModel);
        }

        try
        {
            await _userManagementService.UpdateAsync(viewModel, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var tenants = await _tenantService.GetAllAsync(cancellationToken);
            viewModel.AvailableTenants = tenants.Select(t => new CafeMenu.Application.Models.TenantDto
            {
                TenantId = t.TenantId,
                Name = t.Name,
                Code = t.Code,
                IsActive = t.IsActive,
                CreatedDate = t.CreatedDate
            }).ToList();
            return View(viewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _userManagementService.DeleteAsync(id, tenantId, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}

