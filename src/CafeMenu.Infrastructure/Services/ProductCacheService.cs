using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CafeMenu.Infrastructure.Services;

public class ProductCacheService : IProductCacheService
{
    private readonly IProductRepository _productRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IProductPropertyRepository _productPropertyRepository;
    private readonly IMemoryCache _cache;
    private const int CacheExpirationMinutes = 30;

    public ProductCacheService(
        IProductRepository productRepository,
        IPropertyRepository propertyRepository,
        IProductPropertyRepository productPropertyRepository,
        IMemoryCache cache)
    {
        _productRepository = productRepository;
        _propertyRepository = propertyRepository;
        _productPropertyRepository = productPropertyRepository;
        _cache = cache;
    }

    public async Task<IReadOnlyList<ProductDto>> GetProductsForTenantAsync(int tenantId, bool forceRefresh = false, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"products_tenant_{tenantId}";

        if (!forceRefresh && _cache.TryGetValue(cacheKey, out IReadOnlyList<ProductDto>? cachedProducts) && cachedProducts != null)
        {
            return cachedProducts;
        }

        var products = await _productRepository.GetAllForTenantAsync(tenantId, cancellationToken);
        var allProperties = await _propertyRepository.GetAllForTenantAsync(tenantId, cancellationToken);
        var propertyMap = allProperties.ToDictionary(p => p.PropertyId, p => new PropertyDto
        {
            PropertyId = p.PropertyId,
            Key = p.Key,
            Value = p.Value
        });

        var productDtos = products.Select(p => new ProductDto
        {
            ProductId = p.ProductId,
            ProductName = p.ProductName,
            CategoryId = p.CategoryId,
            CategoryName = p.Category?.CategoryName ?? string.Empty,
            Price = p.Price,
            ImagePath = p.ImagePath,
            Properties = p.ProductProperties
                .Select(pp => propertyMap.GetValueOrDefault(pp.PropertyId))
                .Where(prop => prop != null)
                .Cast<PropertyDto>()
                .ToList()
        }).ToList();

        _cache.Set(cacheKey, productDtos, TimeSpan.FromMinutes(CacheExpirationMinutes));

        return productDtos;
    }

    public void InvalidateTenantCache(int tenantId)
    {
        var cacheKey = $"products_tenant_{tenantId}";
        _cache.Remove(cacheKey);
    }
}

