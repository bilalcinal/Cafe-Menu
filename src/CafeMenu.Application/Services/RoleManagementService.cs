using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CafeMenu.Application.Services;

public class RoleManagementService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoleManagementService(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IRolePermissionRepository rolePermissionRepository,
        ITenantRepository tenantRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _tenantRepository = tenantRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetAllForTenantAsync(tenantId, cancellationToken);
        var tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);
        var tenantName = tenant?.Name ?? string.Empty;

        return roles.Select(r => new RoleDto
        {
            RoleId = r.RoleId,
            Name = r.Name,
            TenantId = r.TenantId,
            TenantName = tenantName,
            IsSystem = r.IsSystem,
            IsActive = r.IsActive,
            CreatedDate = r.CreatedDate,
            UserCount = r.Users.Count(u => !u.IsDeleted)
        }).ToList();
    }

    public async Task<IReadOnlyList<RoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository
            .Query()
            .Include(r => r.Tenant)
            .Include(r => r.Users)
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.TenantId)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);
        var tenantMap = tenants.ToDictionary(t => t.TenantId, t => t.Name);

        return roles.Select(r => new RoleDto
        {
            RoleId = r.RoleId,
            Name = r.Name,
            TenantId = r.TenantId,
            TenantName = tenantMap.GetValueOrDefault(r.TenantId, string.Empty),
            IsSystem = r.IsSystem,
            IsActive = r.IsActive,
            CreatedDate = r.CreatedDate,
            UserCount = r.Users.Count(u => !u.IsDeleted)
        }).ToList();
    }

    public async Task<RoleViewModel?> GetByIdAsync(int roleId, int tenantId, CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionRepository.GetActiveAsync(cancellationToken);
        var availablePermissions = permissions.Select(p => new PermissionDto
        {
            PermissionId = p.PermissionId,
            Key = p.Key,
            DisplayName = p.DisplayName,
            GroupName = p.GroupName,
            IsActive = p.IsActive
        }).ToList();

        var allTenants = await _tenantRepository.GetAllAsync(cancellationToken);

        if (roleId == 0)
        {
            var currentTenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);
            return new RoleViewModel
            {
                RoleId = 0,
                TenantId = tenantId,
                TenantName = currentTenant?.Name ?? string.Empty,
                IsActive = true,
                AvailablePermissions = availablePermissions,
                AvailableTenants = allTenants.Select(t => new TenantDto
                {
                    TenantId = t.TenantId,
                    Name = t.Name,
                    Code = t.Code,
                    IsActive = t.IsActive,
                    CreatedDate = t.CreatedDate
                }).ToList()
            };
        }

        var role = await _roleRepository
            .Query()
            .FirstOrDefaultAsync(r => r.RoleId == roleId && r.TenantId == tenantId && !r.IsDeleted, cancellationToken);
        if (role == null)
        {
            role = await _roleRepository
                .Query()
                .FirstOrDefaultAsync(r => r.RoleId == roleId && !r.IsDeleted, cancellationToken);
            if (role == null)
                return null;
        }

        var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(roleId, cancellationToken);
        var selectedPermissionIds = rolePermissions.Select(rp => rp.PermissionId).ToList();
        
        if (role.Name == "SuperAdmin" && role.IsSystem)
        {
            selectedPermissionIds = availablePermissions.Select(p => p.PermissionId).ToList();
        }
        
        var roleTenant = await _tenantRepository.GetByIdAsync(role.TenantId, cancellationToken);

        return new RoleViewModel
        {
            RoleId = role.RoleId,
            Name = role.Name,
            TenantId = role.TenantId,
            TenantName = roleTenant?.Name ?? string.Empty,
            IsSystem = role.IsSystem,
            IsActive = role.IsActive,
            SelectedPermissionIds = selectedPermissionIds,
            AvailablePermissions = availablePermissions,
            AvailableTenants = allTenants.Select(t => new TenantDto
            {
                TenantId = t.TenantId,
                Name = t.Name,
                Code = t.Code,
                IsActive = t.IsActive,
                CreatedDate = t.CreatedDate
            }).ToList()
        };
    }

    public async Task<int> CreateAsync(RoleViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var currentUserId = GetCurrentUserId();
        var role = new Role
        {
            Name = viewModel.Name,
            TenantId = viewModel.TenantId,
            IsSystem = false,
            IsActive = viewModel.IsActive,
            CreatedDate = DateTime.UtcNow,
            CreatorUserId = currentUserId,
            IsDeleted = false
        };

        await _roleRepository.AddAsync(role, cancellationToken);
        await _roleRepository.SaveChangesAsync(cancellationToken);
        var roleId = role.RoleId;

        await UpdateRolePermissionsAsync(roleId, viewModel.SelectedPermissionIds, cancellationToken);

        return roleId;
    }

    private int? GetCurrentUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return null;

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var userId))
            return userId;

        return null;
    }

    public async Task UpdateAsync(RoleViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository
            .Query()
            .FirstOrDefaultAsync(r => r.RoleId == viewModel.RoleId && r.TenantId == viewModel.TenantId && !r.IsDeleted, cancellationToken);
        if (role == null)
            throw new InvalidOperationException("Rol bulunamadı");

        if (role.IsSystem)
            throw new InvalidOperationException("Sistem rolleri düzenlenemez");

        role.Name = viewModel.Name;
        role.IsActive = viewModel.IsActive;
        _roleRepository.Update(role);
        await _roleRepository.SaveChangesAsync(cancellationToken);

        await UpdateRolePermissionsAsync(viewModel.RoleId, viewModel.SelectedPermissionIds, cancellationToken);
    }

    public async Task DeleteAsync(int roleId, int tenantId, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository
            .Query()
            .FirstOrDefaultAsync(r => r.RoleId == roleId && r.TenantId == tenantId && !r.IsDeleted, cancellationToken);
        if (role == null)
            throw new InvalidOperationException("Rol bulunamadı");

        if (role.IsSystem)
            throw new InvalidOperationException("Sistem rolleri silinemez");

        var hasUsers = await _roleRepository.HasUsersAsync(roleId, cancellationToken);
        if (hasUsers)
            throw new InvalidOperationException("Bu role atanmış kullanıcılar olduğu için rol silinemez");

        role.SoftDelete();
        _roleRepository.Update(role);
        await _roleRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRolePermissionsAsync(int roleId, List<int> permissionIds, CancellationToken cancellationToken = default)
    {
        await _rolePermissionRepository.RemoveAllForRoleAsync(roleId, cancellationToken);
        await _rolePermissionRepository.SaveChangesAsync(cancellationToken);

        foreach (var permissionId in permissionIds)
        {
            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            };
            await _rolePermissionRepository.AddAsync(rolePermission, cancellationToken);
        }
        await _rolePermissionRepository.SaveChangesAsync(cancellationToken);
    }
}

