using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Services;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITenantResolver _tenantResolver;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ITenantResolver tenantResolver)
    {
        _categoryRepository = categoryRepository;
        _tenantResolver = tenantResolver;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var categories = await _categoryRepository.GetAllForTenantAsync(tenantId, cancellationToken);

        return categories
            .Where(c => !c.IsDeleted)
            .Select(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                ParentCategoryId = c.ParentCategoryId,
                ParentCategoryName = c.ParentCategory?.CategoryName
            })
            .ToList();
    }

    public async Task<CategoryViewModel?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var category = await _categoryRepository.GetByIdAsync(categoryId, tenantId, cancellationToken);

        if (category == null || category.IsDeleted)
            return null;

        var allCategories = await GetAllAsync(cancellationToken);

        return new CategoryViewModel
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            ParentCategoryId = category.ParentCategoryId,
            AvailableParentCategories = allCategories.Where(c => c.CategoryId != categoryId).ToList()
        };
    }

    public async Task<int> CreateAsync(CategoryViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();

        var category = new Category
        {
            CategoryName = viewModel.CategoryName,
            ParentCategoryId = viewModel.ParentCategoryId,
            TenantId = tenantId,
            CreatedDate = DateTime.UtcNow,
            IsDeleted = false
        };

        var created = await _categoryRepository.AddAsync(category, cancellationToken);
        return created.CategoryId;
    }

    public async Task UpdateAsync(CategoryViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var category = await _categoryRepository.GetByIdAsync(viewModel.CategoryId, tenantId);

        if (category == null || category.IsDeleted)
            throw new InvalidOperationException("Kategori bulunamadı");

        category.CategoryName = viewModel.CategoryName;
        category.ParentCategoryId = viewModel.ParentCategoryId;

        await _categoryRepository.UpdateAsync(category);
    }

    public async Task DeleteAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        await _categoryRepository.DeleteAsync(categoryId, tenantId, cancellationToken);
    }
}

