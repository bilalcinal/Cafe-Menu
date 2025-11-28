using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfCategoryRepository : ICategoryRepository
{
    private readonly CafeMenuDbContext _context;

    public EfCategoryRepository(CafeMenuDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Category>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .Where(c => c.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .Include(c => c.ParentCategory)
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId && c.TenantId == tenantId, cancellationToken);
    }

    public async Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return category;
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default)
    {
        var category = await GetByIdAsync(categoryId, tenantId, cancellationToken);
        if (category != null)
        {
            category.SoftDelete();
            await UpdateAsync(category, cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Categories
            .AnyAsync(c => c.CategoryId == categoryId && c.TenantId == tenantId && !c.IsDeleted, cancellationToken);
    }
}

