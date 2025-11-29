using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using CafeMenu.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfCategoryRepository : GenericRepository<Category, int>, ICategoryRepository
{
    public EfCategoryRepository(CafeMenuDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Category>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Include(c => c.ParentCategory)
            .Where(c => c.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AnyAsync(c => c.CategoryId == categoryId && c.TenantId == tenantId && !c.IsDeleted, cancellationToken);
    }
}

