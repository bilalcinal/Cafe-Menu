using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
        var category = await _categoryRepository
            .Query()
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.TenantId == tenantId, cancellationToken);

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

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);
        return category.CategoryId;
    }

    public async Task UpdateAsync(CategoryViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var category = await _categoryRepository
            .Query()
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.CategoryId == viewModel.CategoryId && c.TenantId == tenantId, cancellationToken);

        if (category == null || category.IsDeleted)
            throw new InvalidOperationException("Kategori bulunamadı");

        category.CategoryName = viewModel.CategoryName;
        category.ParentCategoryId = viewModel.ParentCategoryId;

        _categoryRepository.Update(category);
        await _categoryRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantResolver.GetCurrentTenantId();
        var category = await _categoryRepository
            .Query()
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.TenantId == tenantId, cancellationToken);
        
        if (category != null)
        {
            category.SoftDelete();
            _categoryRepository.Update(category);
            await _categoryRepository.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoryHierarchyAsync(CancellationToken cancellationToken = default)
    {
        var allCategories = await GetAllAsync(cancellationToken);
        var categoryDict = allCategories.ToDictionary(c => c.CategoryId, c => new CategoryDto
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            ParentCategoryId = c.ParentCategoryId,
            ParentCategoryName = c.ParentCategoryName,
            Children = new List<CategoryDto>()
        });

        var rootCategories = new List<CategoryDto>();

        foreach (var category in categoryDict.Values)
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

        return rootCategories;
    }

    public async Task<List<int>> GetDescendantCategoryIdsAsync(int parentCategoryId, CancellationToken cancellationToken = default)
    {
        var allCategories = await GetAllAsync(cancellationToken);
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
}

