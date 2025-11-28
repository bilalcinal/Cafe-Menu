using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfProductRepository : IProductRepository
{
    private readonly CafeMenuDbContext _context;

    public EfProductRepository(CafeMenuDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductProperties)
                .ThenInclude(pp => pp.Property)
            .Where(p => p.CategoryId == categoryId && p.TenantId == tenantId && !p.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(int productId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductProperties)
                .ThenInclude(pp => pp.Property)
            .FirstOrDefaultAsync(p => p.ProductId == productId && p.TenantId == tenantId, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.ProductProperties)
                .ThenInclude(pp => pp.Property)
            .Where(p => p.TenantId == tenantId && !p.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product> AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int productId, int tenantId, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(productId, tenantId, cancellationToken);
        if (product != null)
        {
            product.SoftDelete();
            await UpdateAsync(product, cancellationToken);
        }
    }

    public async Task<Dictionary<int, int>> GetProductCountByCategoryAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var result = await _context.Products
            .Where(p => p.TenantId == tenantId && !p.IsDeleted)
            .GroupBy(p => p.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return result.ToDictionary(x => x.CategoryId, x => x.Count);
    }
}

