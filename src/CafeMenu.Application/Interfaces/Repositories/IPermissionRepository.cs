using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface IPermissionRepository
{
    Task<Permission?> GetByIdAsync(int permissionId, CancellationToken cancellationToken = default);
    Task<Permission?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Permission>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<int> CreateAsync(Permission permission, CancellationToken cancellationToken = default);
    Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default);
}

