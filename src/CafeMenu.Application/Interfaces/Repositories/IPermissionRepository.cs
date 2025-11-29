using CafeMenu.Application.Interfaces.Repositories.Base;
using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface IPermissionRepository : IRepository<Permission, int>
{
    Task<Permission?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetActiveAsync(CancellationToken cancellationToken = default);
}

