using CafeMenu.Application.Interfaces.Repositories.Base;
using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface IRoleRepository : IRepository<Role, int>
{
    Task<IReadOnlyList<Role>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<Role?> GetByNameAsync(string name, int tenantId, CancellationToken cancellationToken = default);
    Task<bool> HasUsersAsync(int roleId, CancellationToken cancellationToken = default);
}

