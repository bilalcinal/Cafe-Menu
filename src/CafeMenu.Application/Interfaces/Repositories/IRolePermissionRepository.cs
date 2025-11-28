using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface IRolePermissionRepository
{
    Task<IReadOnlyList<RolePermission>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default);
    Task AddAsync(RolePermission rolePermission, CancellationToken cancellationToken = default);
    Task RemoveAsync(int roleId, int permissionId, CancellationToken cancellationToken = default);
    Task RemoveAllForRoleAsync(int roleId, CancellationToken cancellationToken = default);
}

