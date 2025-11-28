using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface ITenantRepository
{
    Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Tenant?> GetByIdAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<Tenant?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<Tenant> AddAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task DeleteAsync(int tenantId, CancellationToken cancellationToken = default);
}

