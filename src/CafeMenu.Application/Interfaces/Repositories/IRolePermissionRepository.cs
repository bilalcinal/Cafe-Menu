using CafeMenu.Application.Interfaces.Repositories.Base;
using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface IRolePermissionRepository : IRepository<RolePermission, int>
{
    Task<IReadOnlyList<RolePermission>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default);
    Task RemoveAllForRoleAsync(int roleId, CancellationToken cancellationToken = default);
}

