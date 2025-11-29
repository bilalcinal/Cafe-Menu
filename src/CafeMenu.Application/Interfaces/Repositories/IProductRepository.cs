using CafeMenu.Application.Interfaces.Repositories.Base;
using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface IProductRepository : IRepository<Product, int>
{
    Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<Dictionary<int, int>> GetProductCountByCategoryAsync(int tenantId, CancellationToken cancellationToken = default);
}

