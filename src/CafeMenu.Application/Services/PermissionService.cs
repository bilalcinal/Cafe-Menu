using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
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
        var user = await _userRepository.GetByIdAsync(userId, currentTenantId, cancellationToken);
        if (user == null || user.IsDeleted)
        {
            if (currentTenantId == 1)
            {
                var allTenants = await GetAllTenantsForUser(userId, cancellationToken);
                foreach (var tenantId in allTenants)
                {
                    user = await _userRepository.GetByIdAsync(userId, tenantId, cancellationToken);
                    if (user != null && !user.IsDeleted)
                        break;
                }
            }
            if (user == null || user.IsDeleted)
                return false;
        }

        if (await IsSuperAdminAsync(userId, cancellationToken))
            return true;

        var role = await _roleRepository.GetByIdAsync(user.RoleId, user.TenantId, cancellationToken);
        if (role == null)
            return false;

        if (role.Name == "TenantAdmin" && await IsTenantAdminAsync(userId, cancellationToken))
            return true;

        var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(role.RoleId, cancellationToken);
        return rolePermissions.Any(rp => rp.Permission?.Key == permissionKey);
    }

    public async Task<IReadOnlyList<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var currentTenantId = GetCurrentTenantId();
        var user = await _userRepository.GetByIdAsync(userId, currentTenantId, cancellationToken);
        if (user == null || user.IsDeleted)
        {
            if (currentTenantId == 1)
            {
                var allTenants = await GetAllTenantsForUser(userId, cancellationToken);
                foreach (var tenantId in allTenants)
                {
                    user = await _userRepository.GetByIdAsync(userId, tenantId, cancellationToken);
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

        var role = await _roleRepository.GetByIdAsync(user.RoleId, user.TenantId, cancellationToken);
        if (role == null)
            return new List<string>();

        if (role.Name == "TenantAdmin")
        {
            var tenantPermissions = await _permissionRepository.GetActiveAsync(cancellationToken);
            return tenantPermissions.Select(p => p.Key).ToList();
        }

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

        var role = await _roleRepository.GetByIdAsync(user.RoleId, user.TenantId, cancellationToken);
        return role?.Name == "SuperAdmin";
    }

    public async Task<bool> IsTenantAdminAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await GetUserFromAnyTenantAsync(userId, cancellationToken);
        if (user == null || user.IsDeleted)
            return false;

        if (user.TenantId == 1)
            return false;

        var role = await _roleRepository.GetByIdAsync(user.RoleId, user.TenantId, cancellationToken);
        return role?.Name == "TenantAdmin";
    }

    private async Task<Domain.Entities.User?> GetUserFromAnyTenantAsync(int userId, CancellationToken cancellationToken)
    {
        var currentTenantId = GetCurrentTenantId();
        var user = await _userRepository.GetByIdAsync(userId, currentTenantId, cancellationToken);
        if (user != null && !user.IsDeleted)
            return user;

        if (currentTenantId == 1)
        {
            var allTenants = await GetAllTenantsForUser(userId, cancellationToken);
            foreach (var tenantId in allTenants)
            {
                user = await _userRepository.GetByIdAsync(userId, tenantId, cancellationToken);
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

