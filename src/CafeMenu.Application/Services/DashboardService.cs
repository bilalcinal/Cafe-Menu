using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models.ViewModels;

namespace CafeMenu.Application.Services;

public class DashboardService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICurrencyService _currencyService;
    private readonly ITenantResolver _tenantResolver;

    public DashboardService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ICurrencyService currencyService,
        ITenantResolver tenantResolver)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _currencyService = currencyService;
        _tenantResolver = tenantResolver;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();

        var productCounts = await _productRepository.GetProductCountByCategoryAsync(tenantId, cancellationToken);
        var categories = await _categoryRepository.GetAllForTenantAsync(tenantId, cancellationToken);

        var categoryNameMap = categories
            .Where(c => !c.IsDeleted)
            .ToDictionary(c => c.CategoryId, c => c.CategoryName);

        var productCountByCategory = productCounts
            .ToDictionary(
                kvp => categoryNameMap.GetValueOrDefault(kvp.Key, $"Kategori {kvp.Key}"),
                kvp => kvp.Value
            );

        var currencyRates = await _currencyService.GetLatestRatesAsync(cancellationToken);

        return new DashboardViewModel
        {
            ProductCountByCategory = productCountByCategory,
            CurrencyRates = currencyRates
        };
    }
}

