using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CafeMenu.Application.Services;

public class PermissionService : IPermissionService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IPermissionRepository permissionRepository,
        ITenantRepository tenantRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _permissionRepository = permissionRepository;
        _tenantRepository = tenantRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> HasPermissionAsync(int userId, string permissionKey, CancellationToken cancellationToken = default)
    {
        var currentTenantId = GetCurrentTenantId();
        var user = await _userRepository
            .Query()
            .FirstOrDefaultAsync(u => u.UserId == userId && u.TenantId == currentTenantId, cancellationToken);
        if (user == null || user.IsDeleted)
        {
            if (currentTenantId == 1)
            {
                var allTenants = await GetAllTenantsForUser(userId, cancellationToken);
                foreach (var tenantId in allTenants)
                {
                    user = await _userRepository
                        .Query()
                        .FirstOrDefaultAsync(u => u.UserId == userId && u.TenantId == tenantId, cancellationToken);
                    if (user != null && !user.IsDeleted)
                        break;
                }
            }
            if (user == null || user.IsDeleted)
                return false;
        }

        if (await IsSuperAdminAsync(userId, cancellationToken))
            return true;

        var role = await _roleRepository
            .Query()
            .FirstOrDefaultAsync(r => r.RoleId == user.RoleId && r.TenantId == user.TenantId && !r.IsDeleted, cancellationToken);
        if (role == null)
            return false;

        var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(role.RoleId, cancellationToken);
        return rolePermissions.Any(rp => rp.Permission?.Key == permissionKey);
    }

    public async Task<IReadOnlyList<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var currentTenantId = GetCurrentTenantId();
        var user = await _userRepository
            .Query()
            .FirstOrDefaultAsync(u => u.UserId == userId && u.TenantId == currentTenantId, cancellationToken);
        if (user == null || user.IsDeleted)
        {
            if (currentTenantId == 1)
            {
                var allTenants = await GetAllTenantsForUser(userId, cancellationToken);
                foreach (var tenantId in allTenants)
                {
                    user = await _userRepository
                        .Query()
                        .FirstOrDefaultAsync(u => u.UserId == userId && u.TenantId == tenantId, cancellationToken);
                    if (user != null && !user.IsDeleted)
                        break;
                }
            }
            if (user == null || user.IsDeleted)
                return new List<string>();
        }

        if (await IsSuperAdminAsync(userId, cancellationToken))
        {
            var allPermissions = await _permissionRepository.GetActiveAsync(cancellationToken);
            return allPermissions.Select(p => p.Key).ToList();
        }

        var role = await _roleRepository
            .Query()
            .FirstOrDefaultAsync(r => r.RoleId == user.RoleId && r.TenantId == user.TenantId && !r.IsDeleted, cancellationToken);
        if (role == null)
            return new List<string>();

        var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(role.RoleId, cancellationToken);
        return rolePermissions.Select(rp => rp.Permission?.Key ?? string.Empty)
            .Where(k => !string.IsNullOrEmpty(k))
            .ToList();
    }

    public async Task<bool> IsSuperAdminAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await GetUserFromAnyTenantAsync(userId, cancellationToken);
        if (user == null || user.IsDeleted)
            return false;

        if (user.TenantId != 1)
            return false;

        var role = await _roleRepository
            .Query()
            .FirstOrDefaultAsync(r => r.RoleId == user.RoleId && r.TenantId == user.TenantId && !r.IsDeleted, cancellationToken);
        return role?.Name == "SuperAdmin";
    }

    private async Task<Domain.Entities.User?> GetUserFromAnyTenantAsync(int userId, CancellationToken cancellationToken)
    {
        var currentTenantId = GetCurrentTenantId();
        var user = await _userRepository
            .Query()
            .FirstOrDefaultAsync(u => u.UserId == userId && u.TenantId == currentTenantId, cancellationToken);
        if (user != null && !user.IsDeleted)
            return user;

        if (currentTenantId == 1)
        {
            var allTenants = await GetAllTenantsForUser(userId, cancellationToken);
            foreach (var tenantId in allTenants)
            {
                user = await _userRepository
                    .Query()
                    .FirstOrDefaultAsync(u => u.UserId == userId && u.TenantId == tenantId, cancellationToken);
                if (user != null && !user.IsDeleted)
                    return user;
            }
        }

        return null;
    }

    private async Task<List<int>> GetAllTenantsForUser(int userId, CancellationToken cancellationToken)
    {
        var allTenants = await _tenantRepository.GetAllAsync(cancellationToken);
        return allTenants.Select(t => t.TenantId).ToList();
    }

    public async Task<bool> HasCurrentUserPermissionAsync(string permissionKey, CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return false;

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return false;

        return await HasPermissionAsync(userId, permissionKey, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetCurrentUserPermissionsAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return new List<string>();

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return new List<string>();

        return await GetUserPermissionsAsync(userId, cancellationToken);
    }

    public bool IsCurrentUserSuperAdmin()
    {
        var role = GetCurrentUserRole();
        return role == "SuperAdmin";
    }

    public string? GetCurrentUserRole()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return null;

        return httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
    }

    public int GetCurrentTenantId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return 1;

        var tenantIdClaim = httpContext.User.FindFirst("TenantId")?.Value;
        if (!string.IsNullOrEmpty(tenantIdClaim) && int.TryParse(tenantIdClaim, out var tenantId))
            return tenantId;

        return 1;
    }
}

