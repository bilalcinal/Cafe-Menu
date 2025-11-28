using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPropertyRepository _propertyRepository;
    private readonly IProductPropertyRepository _productPropertyRepository;
    private readonly IProductCacheService _productCacheService;
    private readonly ITenantResolver _tenantResolver;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IPropertyRepository propertyRepository,
        IProductPropertyRepository productPropertyRepository,
        IProductCacheService productCacheService,
        ITenantResolver tenantResolver)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _propertyRepository = propertyRepository;
        _productPropertyRepository = productPropertyRepository;
        _productCacheService = productCacheService;
        _tenantResolver = tenantResolver;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        return await _productCacheService.GetProductsForTenantAsync(tenantId, false, cancellationToken);
    }

    public async Task<ProductViewModel?> GetByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var product = await _productRepository.GetByIdAsync(productId, tenantId, cancellationToken);

        if (product == null || product.IsDeleted)
            return null;

        var categories = await _categoryRepository.GetAllForTenantAsync(tenantId, cancellationToken);
        var properties = await _propertyRepository.GetAllForTenantAsync(tenantId, cancellationToken);
        var productProperties = await _productPropertyRepository.GetByProductIdAsync(productId, cancellationToken);

        return new ProductViewModel
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            CategoryId = product.CategoryId,
            Price = product.Price,
            ImagePath = product.ImagePath,
            AvailableCategories = categories
                .Where(c => !c.IsDeleted)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName
                })
                .ToList(),
            AvailableProperties = properties
                .Select(p => new PropertyDto
                {
                    PropertyId = p.PropertyId,
                    Key = p.Key,
                    Value = p.Value
                })
                .ToList(),
            SelectedPropertyIds = productProperties.Select(pp => pp.PropertyId).ToList()
        };
    }

    public async Task<int> CreateAsync(ProductViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();

        var product = new Product
        {
            ProductName = viewModel.ProductName,
            CategoryId = viewModel.CategoryId,
            Price = viewModel.Price,
            ImagePath = viewModel.ImagePath,
            TenantId = tenantId,
            CreatedDate = DateTime.UtcNow,
            IsDeleted = false
        };

        var created = await _productRepository.AddAsync(product, cancellationToken);

        foreach (var propertyId in viewModel.SelectedPropertyIds)
        {
            var productProperty = new ProductProperty
            {
                ProductId = created.ProductId,
                PropertyId = propertyId
            };
            await _productPropertyRepository.AddAsync(productProperty, cancellationToken);
        }

        _productCacheService.InvalidateTenantCache(tenantId);

        return created.ProductId;
    }

    public async Task UpdateAsync(ProductViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var product = await _productRepository.GetByIdAsync(viewModel.ProductId, tenantId);

        if (product == null || product.IsDeleted)
            throw new InvalidOperationException("Ürün bulunamadı");

        product.ProductName = viewModel.ProductName;
        product.CategoryId = viewModel.CategoryId;
        product.Price = viewModel.Price;
        product.ImagePath = viewModel.ImagePath;

        await _productRepository.UpdateAsync(product);

        await _productPropertyRepository.DeleteByProductIdAsync(product.ProductId, cancellationToken);

        foreach (var propertyId in viewModel.SelectedPropertyIds)
        {
            var productProperty = new ProductProperty
            {
                ProductId = product.ProductId,
                PropertyId = propertyId
            };
            await _productPropertyRepository.AddAsync(productProperty, cancellationToken);
        }

        _productCacheService.InvalidateTenantCache(tenantId);
    }

    public async Task DeleteAsync(int productId, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        await _productRepository.DeleteAsync(productId, tenantId, cancellationToken);
        _productCacheService.InvalidateTenantCache(tenantId);
    }
}

