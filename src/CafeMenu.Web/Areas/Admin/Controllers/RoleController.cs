using CafeMenu.Application.Services;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Web.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CafeMenu.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class RoleController : Controller
{
    private readonly RoleManagementService _roleManagementService;
    private readonly IPermissionService _permissionService;

    public RoleController(
        RoleManagementService roleManagementService,
        IPermissionService permissionService)
    {
        _roleManagementService = roleManagementService;
        _permissionService = permissionService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var currentRole = _permissionService.GetCurrentUserRole();
        var currentTenantId = _permissionService.GetCurrentTenantId();

        if (currentRole != "SuperAdmin" && currentRole != "TenantAdmin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        IReadOnlyList<RoleDto> roles;
        if (currentRole == "SuperAdmin")
        {
            roles = await _roleManagementService.GetAllAsync(cancellationToken);
        }
        else
        {
            roles = await _roleManagementService.GetAllForTenantAsync(currentTenantId, cancellationToken);
        }

        return View(roles);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
    {
        var currentRole = _permissionService.GetCurrentUserRole();
        var currentTenantId = _permissionService.GetCurrentTenantId();

        if (currentRole != "SuperAdmin" && currentRole != "TenantAdmin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var viewModel = await _roleManagementService.GetByIdAsync(0, currentTenantId, cancellationToken);
        if (viewModel == null)
        {
            return NotFound();
        }

        if (currentRole != "SuperAdmin")
        {
            viewModel.AvailableTenants = viewModel.AvailableTenants.Where(t => t.TenantId == currentTenantId).ToList();
        }

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RoleViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var currentRole = _permissionService.GetCurrentUserRole();
        var currentTenantId = _permissionService.GetCurrentTenantId();

        if (currentRole != "SuperAdmin" && currentRole != "TenantAdmin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        if (currentRole != "SuperAdmin")
        {
            viewModel.TenantId = currentTenantId;
        }

        if (!ModelState.IsValid)
        {
            var tempViewModel = await _roleManagementService.GetByIdAsync(0, viewModel.TenantId, cancellationToken);
            if (tempViewModel != null)
            {
                viewModel.AvailablePermissions = tempViewModel.AvailablePermissions;
                viewModel.AvailableTenants = tempViewModel.AvailableTenants;
                if (currentRole != "SuperAdmin")
                {
                    viewModel.AvailableTenants = viewModel.AvailableTenants.Where(t => t.TenantId == currentTenantId).ToList();
                }
            }
            return View(viewModel);
        }

        try
        {
            await _roleManagementService.CreateAsync(viewModel, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var tempViewModel = await _roleManagementService.GetByIdAsync(0, viewModel.TenantId, cancellationToken);
            if (tempViewModel != null)
            {
                viewModel.AvailablePermissions = tempViewModel.AvailablePermissions;
                viewModel.AvailableTenants = tempViewModel.AvailableTenants;
                if (currentRole != "SuperAdmin")
                {
                    viewModel.AvailableTenants = viewModel.AvailableTenants.Where(t => t.TenantId == currentTenantId).ToList();
                }
            }
            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken = default)
    {
        var currentRole = _permissionService.GetCurrentUserRole();
        var currentTenantId = _permissionService.GetCurrentTenantId();

        if (currentRole != "SuperAdmin" && currentRole != "TenantAdmin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var roleViewModel = await _roleManagementService.GetByIdAsync(id, currentTenantId, cancellationToken);
        if (roleViewModel == null)
        {
            return NotFound();
        }

        var editViewModel = new RoleEditViewModel
        {
            RoleId = roleViewModel.RoleId,
            Name = roleViewModel.Name,
            TenantId = roleViewModel.TenantId,
            TenantName = roleViewModel.TenantName,
            IsSystem = roleViewModel.IsSystem,
            IsActive = roleViewModel.IsActive
        };

        return View(editViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RoleEditViewModel model, CancellationToken cancellationToken = default)
    {
        var currentRole = _permissionService.GetCurrentUserRole();
        var currentTenantId = _permissionService.GetCurrentTenantId();

        if (currentRole != "SuperAdmin" && currentRole != "TenantAdmin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        if (!ModelState.IsValid)
        {
            var existingViewModel = await _roleManagementService.GetByIdAsync(model.RoleId, currentTenantId, cancellationToken);
            if (existingViewModel != null)
            {
                model.TenantName = existingViewModel.TenantName;
                model.IsSystem = existingViewModel.IsSystem;
            }
            return View(model);
        }

        try
        {
            var roleViewModel = new RoleViewModel
            {
                RoleId = model.RoleId,
                Name = model.Name,
                TenantId = model.TenantId,
                IsActive = model.IsActive,
                SelectedPermissionIds = new List<int>()
            };
            await _roleManagementService.UpdateAsync(roleViewModel, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var existingViewModel = await _roleManagementService.GetByIdAsync(model.RoleId, currentTenantId, cancellationToken);
            if (existingViewModel != null)
            {
                model.TenantName = existingViewModel.TenantName;
                model.IsSystem = existingViewModel.IsSystem;
            }
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var currentRole = _permissionService.GetCurrentUserRole();
        var currentTenantId = _permissionService.GetCurrentTenantId();

        if (currentRole != "SuperAdmin" && currentRole != "TenantAdmin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        try
        {
            await _roleManagementService.DeleteAsync(id, currentTenantId, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Permissions(int id, CancellationToken cancellationToken = default)
    {
        var currentRole = _permissionService.GetCurrentUserRole();
        var currentTenantId = _permissionService.GetCurrentTenantId();

        if (currentRole != "SuperAdmin" && currentRole != "TenantAdmin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var roleViewModel = await _roleManagementService.GetByIdAsync(id, currentTenantId, cancellationToken);
        if (roleViewModel == null)
        {
            return NotFound();
        }

        if (currentRole != "SuperAdmin" && roleViewModel.TenantId != currentTenantId)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        if (roleViewModel.IsSystem && roleViewModel.Name == "SuperAdmin")
        {
            TempData["InfoMessage"] = "SuperAdmin rolü tüm izinlere sahiptir ve düzenlenemez.";
        }

        var permissionsViewModel = new RolePermissionsViewModel
        {
            RoleId = roleViewModel.RoleId,
            Name = roleViewModel.Name,
            TenantId = roleViewModel.TenantId,
            TenantName = roleViewModel.TenantName,
            IsSystem = roleViewModel.IsSystem,
            SelectedPermissionIds = roleViewModel.SelectedPermissionIds,
            AvailablePermissions = roleViewModel.AvailablePermissions
        };

        return View(permissionsViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Permissions(RolePermissionsViewModel model, CancellationToken cancellationToken = default)
    {
        var currentRole = _permissionService.GetCurrentUserRole();
        var currentTenantId = _permissionService.GetCurrentTenantId();

        if (currentRole != "SuperAdmin" && currentRole != "TenantAdmin")
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var existingRole = await _roleManagementService.GetByIdAsync(model.RoleId, currentTenantId, cancellationToken);
        if (existingRole == null)
        {
            return NotFound();
        }

        if (currentRole != "SuperAdmin" && existingRole.TenantId != currentTenantId)
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        if (existingRole.IsSystem && existingRole.Name == "SuperAdmin")
        {
            TempData["ErrorMessage"] = "SuperAdmin rolü düzenlenemez.";
            return RedirectToAction(nameof(Permissions), new { id = model.RoleId });
        }

        try
        {
            await _roleManagementService.UpdateRolePermissionsAsync(model.RoleId, model.SelectedPermissionIds, cancellationToken);
            TempData["SuccessMessage"] = "İzinler başarıyla güncellendi.";
            return RedirectToAction(nameof(Permissions), new { id = model.RoleId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var updatedViewModel = await _roleManagementService.GetByIdAsync(model.RoleId, currentTenantId, cancellationToken);
            if (updatedViewModel != null)
            {
                model.Name = updatedViewModel.Name;
                model.TenantId = updatedViewModel.TenantId;
                model.TenantName = updatedViewModel.TenantName;
                model.IsSystem = updatedViewModel.IsSystem;
                model.AvailablePermissions = updatedViewModel.AvailablePermissions;
            }
            return View(model);
        }
    }
}

