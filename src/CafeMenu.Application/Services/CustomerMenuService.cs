using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models;
using CafeMenu.Application.Models.ViewModels;

namespace CafeMenu.Application.Services;

public class CustomerMenuService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductCacheService _productCacheService;
    private readonly ICurrencyService _currencyService;
    private readonly ITenantResolver _tenantResolver;

    public CustomerMenuService(
        ICategoryRepository categoryRepository,
        IProductCacheService productCacheService,
        ICurrencyService currencyService,
        ITenantResolver tenantResolver)
    {
        _categoryRepository = categoryRepository;
        _productCacheService = productCacheService;
        _currencyService = currencyService;
        _tenantResolver = tenantResolver;
    }

    public async Task<CustomerMenuViewModel> GetMenuAsync(int? categoryId, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();

        var categories = await _categoryRepository.GetAllForTenantAsync(tenantId, cancellationToken);
        var categoryDtos = categories
            .Where(c => !c.IsDeleted)
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory?.CategoryName
            })
            .ToList();

        var allProducts = await _productCacheService.GetProductsForTenantAsync(tenantId, false, cancellationToken);

        var products = categoryId.HasValue
            ? allProducts.Where(p => p.CategoryId == categoryId.Value).ToList()
            : allProducts.ToList();

        var currencyRates = await _currencyService.GetLatestRatesAsync(cancellationToken);

        return new CustomerMenuViewModel
        {
            TenantId = tenantId,
            Categories = categoryDtos,
            SelectedCategoryId = categoryId,
            Products = products,
            CurrencyRates = currencyRates
        };
    }
}

