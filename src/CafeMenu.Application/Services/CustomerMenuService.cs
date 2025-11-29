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
    private readonly CategoryService _categoryService;

    public CustomerMenuService(
        ICategoryRepository categoryRepository,
        IProductCacheService productCacheService,
        ICurrencyService currencyService,
        ITenantResolver tenantResolver,
        CategoryService categoryService)
    {
        _categoryRepository = categoryRepository;
        _productCacheService = productCacheService;
        _currencyService = currencyService;
        _tenantResolver = tenantResolver;
        _categoryService = categoryService;
    }

    public async Task<CustomerMenuViewModel> GetMenuAsync(int? categoryId, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();

        var categoryHierarchy = await _categoryService.GetCategoryHierarchyAsync(cancellationToken);
        var allCategories = await _categoryService.GetAllAsync(cancellationToken);

        var allProducts = await _productCacheService.GetProductsForTenantAsync(tenantId, false, cancellationToken);

        List<ProductDto> products;
        string? selectedCategoryName = null;

        if (categoryId.HasValue)
        {
            var selectedCategory = allCategories.FirstOrDefault(c => c.CategoryId == categoryId.Value);
            selectedCategoryName = selectedCategory?.CategoryName;

            var selectedCategoryInHierarchy = categoryHierarchy
                .SelectMany(GetAllDescendants)
                .FirstOrDefault(c => c.CategoryId == categoryId.Value);

            var hasChildren = selectedCategoryInHierarchy != null && selectedCategoryInHierarchy.Children.Any();

            if (hasChildren)
            {
                var descendantIds = await _categoryService.GetDescendantCategoryIdsAsync(categoryId.Value, cancellationToken);
                products = allProducts.Where(p => descendantIds.Contains(p.CategoryId)).ToList();
            }
            else
            {
                products = allProducts.Where(p => p.CategoryId == categoryId.Value).ToList();
            }
        }
        else
        {
            products = allProducts.ToList();
        }

        var currencyRates = await _currencyService.GetLatestRatesAsync(cancellationToken);

        return new CustomerMenuViewModel
        {
            TenantId = tenantId,
            Categories = categoryHierarchy,
            SelectedCategoryId = categoryId,
            SelectedCategoryName = selectedCategoryName,
            Products = products,
            CurrencyRates = currencyRates
        };
    }

    private IEnumerable<CategoryDto> GetAllDescendants(CategoryDto category)
    {
        yield return category;
        foreach (var child in category.Children)
        {
            foreach (var descendant in GetAllDescendants(child))
            {
                yield return descendant;
            }
        }
    }
}

