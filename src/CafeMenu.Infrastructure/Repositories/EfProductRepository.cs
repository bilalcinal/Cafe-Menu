using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using CafeMenu.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfProductRepository : GenericRepository<Product, int>, IProductRepository
{
    public EfProductRepository(CafeMenuDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Include(p => p.Category)
            .Include(p => p.ProductProperties)
                .ThenInclude(pp => pp.Property)
            .Where(p => p.CategoryId == categoryId && p.TenantId == tenantId && !p.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Include(p => p.Category)
            .Include(p => p.ProductProperties)
                .ThenInclude(pp => pp.Property)
            .Where(p => p.TenantId == tenantId && !p.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<int, int>> GetProductCountByCategoryAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var result = await Query()
            .Where(p => p.TenantId == tenantId && !p.IsDeleted)
            .GroupBy(p => p.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return result.ToDictionary(x => x.CategoryId, x => x.Count);
    }
}

