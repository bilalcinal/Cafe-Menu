using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int productId, int tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(int productId, int tenantId, CancellationToken cancellationToken = default);
    Task<Dictionary<int, int>> GetProductCountByCategoryAsync(int tenantId, CancellationToken cancellationToken = default);
}

