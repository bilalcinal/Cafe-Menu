using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int roleId, int tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Role?> GetByNameAsync(string name, int tenantId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Role role, CancellationToken cancellationToken = default);
    Task UpdateAsync(Role role, CancellationToken cancellationToken = default);
    Task DeleteAsync(int roleId, int tenantId, CancellationToken cancellationToken = default);
    Task<bool> HasUsersAsync(int roleId, CancellationToken cancellationToken = default);
}

