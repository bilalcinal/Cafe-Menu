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

    public async Task<MenuViewModel> GetCafeMenuAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var categoryHierarchy = await _categoryService.GetCategoryHierarchyAsync(cancellationToken);
        var allProducts = await _productCacheService.GetProductsForTenantAsync(tenantId, false, cancellationToken);

        var menuCategories = new List<MenuCategoryViewModel>();

        foreach (var category in categoryHierarchy)
        {
            var categoryProducts = allProducts.Where(p => p.CategoryId == category.CategoryId).ToList();
            var subCategories = new List<MenuCategoryViewModel>();

            foreach (var subCategory in category.Children)
            {
                var subCategoryProducts = allProducts.Where(p => p.CategoryId == subCategory.CategoryId).ToList();
                var grandSubCategories = new List<MenuCategoryViewModel>();

                foreach (var grandSubCategory in subCategory.Children)
                {
                    var grandSubCategoryProducts = allProducts.Where(p => p.CategoryId == grandSubCategory.CategoryId).ToList();
                    grandSubCategories.Add(new MenuCategoryViewModel
                    {
                        CategoryId = grandSubCategory.CategoryId,
                        CategoryName = grandSubCategory.CategoryName,
                        ParentCategoryId = grandSubCategory.ParentCategoryId,
                        Products = grandSubCategoryProducts.Select(p => new MenuProductViewModel
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            Price = p.Price,
                            ImagePath = p.ImagePath,
                            Badges = p.Properties.Select(prop => $"{prop.Key}: {prop.Value}").ToList()
                        }).ToList()
                    });
                }

                subCategories.Add(new MenuCategoryViewModel
                {
                    CategoryId = subCategory.CategoryId,
                    CategoryName = subCategory.CategoryName,
                    ParentCategoryId = subCategory.ParentCategoryId,
                    SubCategories = grandSubCategories,
                    Products = subCategoryProducts.Select(p => new MenuProductViewModel
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Price = p.Price,
                        ImagePath = p.ImagePath,
                        Badges = p.Properties.Select(prop => $"{prop.Key}: {prop.Value}").ToList()
                    }).ToList()
                });
            }

            menuCategories.Add(new MenuCategoryViewModel
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                ParentCategoryId = category.ParentCategoryId,
                SubCategories = subCategories,
                Products = categoryProducts.Select(p => new MenuProductViewModel
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ImagePath = p.ImagePath,
                    Badges = p.Properties.Select(prop => $"{prop.Key}: {prop.Value}").ToList()
                }).ToList()
            });
        }

        return new MenuViewModel
        {
            TenantId = tenantId,
            TenantName = string.Empty,
            Categories = menuCategories
        };
    }
}

