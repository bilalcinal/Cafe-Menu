using CafeMenu.Application.Interfaces.Repositories.Base;
using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface ICategoryRepository : IRepository<Category, int>
{
    Task<IReadOnlyList<Category>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default);
}

