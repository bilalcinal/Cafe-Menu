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

        var allCategories = await _categoryRepository.GetAllForTenantAsync(tenantId, cancellationToken);
        var categoryDtos = allCategories
            .Where(c => !c.IsDeleted)
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory?.CategoryName,
                Children = new List<CategoryDto>()
            })
            .ToList();

        var categoryDict = categoryDtos.ToDictionary(c => c.CategoryId, c => c);
        var rootCategories = new List<CategoryDto>();

        foreach (var category in categoryDtos)
        {
            if (category.ParentCategoryId.HasValue && categoryDict.ContainsKey(category.ParentCategoryId.Value))
            {
                categoryDict[category.ParentCategoryId.Value].Children.Add(category);
            }
            else
            {
                rootCategories.Add(category);
            }
        }

        var categoryHierarchy = rootCategories;

        var allProducts = await _productCacheService.GetProductsForTenantAsync(tenantId, false, cancellationToken);

        List<ProductDto> products;
        string? selectedCategoryName = null;

        if (categoryId.HasValue)
        {
            var selectedCategory = categoryDtos.FirstOrDefault(c => c.CategoryId == categoryId.Value);
            selectedCategoryName = selectedCategory?.CategoryName;

            var selectedCategoryInHierarchy = categoryHierarchy
                .SelectMany(GetAllDescendants)
                .FirstOrDefault(c => c.CategoryId == categoryId.Value);

            var hasChildren = selectedCategoryInHierarchy != null && selectedCategoryInHierarchy.Children.Any();

            if (hasChildren)
            {
                var descendantIds = GetDescendantCategoryIds(categoryId.Value, categoryDtos);
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

    private List<int> GetDescendantCategoryIds(int parentCategoryId, List<CategoryDto> allCategories)
    {
        var descendantIds = new List<int> { parentCategoryId };

        void CollectChildren(int categoryId)
        {
            var children = allCategories.Where(c => c.ParentCategoryId == categoryId).ToList();
            foreach (var child in children)
            {
                descendantIds.Add(child.CategoryId);
                CollectChildren(child.CategoryId);
            }
        }

        CollectChildren(parentCategoryId);
        return descendantIds;
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
        
        var allCategories = await _categoryRepository.GetAllForTenantAsync(tenantId, cancellationToken);
        var categoryDtos = allCategories
            .Where(c => !c.IsDeleted)
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory?.CategoryName,
                Children = new List<CategoryDto>()
            })
            .ToList();

        var categoryDict = categoryDtos.ToDictionary(c => c.CategoryId, c => c);
        var rootCategories = new List<CategoryDto>();

        foreach (var category in categoryDtos)
        {
            if (category.ParentCategoryId.HasValue && categoryDict.ContainsKey(category.ParentCategoryId.Value))
            {
                categoryDict[category.ParentCategoryId.Value].Children.Add(category);
            }
            else
            {
                rootCategories.Add(category);
            }
        }

        var categoryHierarchy = rootCategories;
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

