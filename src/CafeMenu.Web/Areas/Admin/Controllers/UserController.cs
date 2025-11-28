using CafeMenu.Application.Services;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeMenu.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class UserController : Controller
{
    private readonly UserManagementService _userManagementService;
    private readonly TenantService _tenantService;
    private readonly ITenantRepository _tenantRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionService _permissionService;

    public UserController(
        UserManagementService userManagementService,
        TenantService tenantService,
        ITenantRepository tenantRepository,
        IRoleRepository roleRepository,
        IPermissionService permissionService)
    {
        _userManagementService = userManagementService;
        _tenantService = tenantService;
        _tenantRepository = tenantRepository;
        _roleRepository = roleRepository;
        _permissionService = permissionService;
    }

    [RequirePermission("User.View")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var currentRole = _permissionService.GetCurrentUserRole();
        if (currentRole != "SuperAdmin" && currentRole != "TenantAdmin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var users = await _userManagementService.GetAllAsync(cancellationToken);
        return View(users);
    }

    [HttpGet]
    [RequirePermission("User.Create")]
    public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
    {
        var currentRole = _permissionService.GetCurrentUserRole();
        var currentTenantId = _permissionService.GetCurrentTenantId();

        var viewModel = new UserViewModel();

        if (currentRole == "SuperAdmin")
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
        }
        else
        {
            var tenant = await _tenantRepository.GetByIdAsync(currentTenantId, cancellationToken);
            if (tenant != null)
            {
                viewModel.AvailableTenants = new List<CafeMenu.Application.Models.TenantDto>
                {
                    new CafeMenu.Application.Models.TenantDto
                    {
                        TenantId = tenant.TenantId,
                        Name = tenant.Name,
                        Code = tenant.Code,
                        IsActive = tenant.IsActive,
                        CreatedDate = tenant.CreatedDate
                    }
                };
            }
            viewModel.TenantId = currentTenantId;
        }

        var roles = await _roleRepository.GetAllForTenantAsync(viewModel.TenantId > 0 ? viewModel.TenantId : currentTenantId, cancellationToken);
        viewModel.AvailableRoles = roles.Where(r => !r.IsSystem || r.Name != "SuperAdmin" || currentRole == "SuperAdmin").Select(r => new CafeMenu.Application.Models.RoleDto
        {
            RoleId = r.RoleId,
            Name = r.Name,
            TenantId = r.TenantId,
            TenantName = string.Empty,
            IsSystem = r.IsSystem,
            IsActive = r.IsActive,
            CreatedDate = r.CreatedDate,
            UserCount = 0
        }).ToList();

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var currentRole = _permissionService.GetCurrentUserRole();
        var currentTenantId = _permissionService.GetCurrentTenantId();

        if (!ModelState.IsValid)
        {
            if (currentRole == "SuperAdmin")
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
            }
            else
            {
                var tenant = await _tenantRepository.GetByIdAsync(currentTenantId, cancellationToken);
                if (tenant != null)
                {
                    viewModel.AvailableTenants = new List<CafeMenu.Application.Models.TenantDto>
                    {
                        new CafeMenu.Application.Models.TenantDto
                        {
                            TenantId = tenant.TenantId,
                            Name = tenant.Name,
                            Code = tenant.Code,
                            IsActive = tenant.IsActive,
                            CreatedDate = tenant.CreatedDate
                        }
                    };
                }
            }

            var roles = await _roleRepository.GetAllForTenantAsync(viewModel.TenantId > 0 ? viewModel.TenantId : currentTenantId, cancellationToken);
            viewModel.AvailableRoles = roles.Where(r => !r.IsSystem || r.Name != "SuperAdmin" || currentRole == "SuperAdmin").Select(r => new CafeMenu.Application.Models.RoleDto
            {
                RoleId = r.RoleId,
                Name = r.Name,
                TenantId = r.TenantId,
                TenantName = string.Empty,
                IsSystem = r.IsSystem,
                IsActive = r.IsActive,
                CreatedDate = r.CreatedDate,
                UserCount = 0
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
            if (currentRole == "SuperAdmin")
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
            }
            else
            {
                var tenant = await _tenantRepository.GetByIdAsync(currentTenantId, cancellationToken);
                if (tenant != null)
                {
                    viewModel.AvailableTenants = new List<CafeMenu.Application.Models.TenantDto>
                    {
                        new CafeMenu.Application.Models.TenantDto
                        {
                            TenantId = tenant.TenantId,
                            Name = tenant.Name,
                            Code = tenant.Code,
                            IsActive = tenant.IsActive,
                            CreatedDate = tenant.CreatedDate
                        }
                    };
                }
            }

            var roles = await _roleRepository.GetAllForTenantAsync(viewModel.TenantId > 0 ? viewModel.TenantId : currentTenantId, cancellationToken);
            viewModel.AvailableRoles = roles.Where(r => !r.IsSystem || r.Name != "SuperAdmin" || currentRole == "SuperAdmin").Select(r => new CafeMenu.Application.Models.RoleDto
            {
                RoleId = r.RoleId,
                Name = r.Name,
                TenantId = r.TenantId,
                TenantName = string.Empty,
                IsSystem = r.IsSystem,
                IsActive = r.IsActive,
                CreatedDate = r.CreatedDate,
                UserCount = 0
            }).ToList();

            return View(viewModel);
        }
    }

    [HttpGet]
    [RequirePermission("User.Edit")]
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
        var currentRole = _permissionService.GetCurrentUserRole();
        var currentTenantId = _permissionService.GetCurrentTenantId();

        if (!ModelState.IsValid)
        {
            var existingViewModel = await _userManagementService.GetByIdAsync(viewModel.UserId, viewModel.TenantId, cancellationToken);
            if (existingViewModel != null)
            {
                viewModel.AvailableTenants = existingViewModel.AvailableTenants;
                viewModel.AvailableRoles = existingViewModel.AvailableRoles;
            }
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
            var existingViewModel = await _userManagementService.GetByIdAsync(viewModel.UserId, viewModel.TenantId, cancellationToken);
            if (existingViewModel != null)
            {
                viewModel.AvailableTenants = existingViewModel.AvailableTenants;
                viewModel.AvailableRoles = existingViewModel.AvailableRoles;
            }
            return View(viewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("User.Delete")]
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

