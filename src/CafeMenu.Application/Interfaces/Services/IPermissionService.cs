namespace CafeMenu.Application.Interfaces.Services;

public interface IPermissionService
{
    Task<bool> HasPermissionAsync(int userId, string permissionKey, CancellationToken cancellationToken = default);
    Task<bool> HasCurrentUserPermissionAsync(string permissionKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetCurrentUserPermissionsAsync(CancellationToken cancellationToken = default);
    Task<bool> IsSuperAdminAsync(int userId, CancellationToken cancellationToken = default);
    bool IsCurrentUserSuperAdmin();
    string? GetCurrentUserRole();
    int GetCurrentTenantId();
}

