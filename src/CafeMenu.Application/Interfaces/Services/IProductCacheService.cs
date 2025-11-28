using CafeMenu.Application.Models;

namespace CafeMenu.Application.Interfaces.Services;

public interface IProductCacheService
{
    Task<IReadOnlyList<ProductDto>> GetProductsForTenantAsync(int tenantId, bool forceRefresh = false, CancellationToken cancellationToken = default);
    void InvalidateTenantCache(int tenantId);
}

