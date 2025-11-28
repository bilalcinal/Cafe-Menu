using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfProductPropertyRepository : IProductPropertyRepository
{
    private readonly CafeMenuDbContext _context;

    public EfProductPropertyRepository(CafeMenuDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ProductProperty>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductProperties
            .Include(pp => pp.Property)
            .Where(pp => pp.ProductId == productId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductProperty> AddAsync(ProductProperty productProperty, CancellationToken cancellationToken = default)
    {
        await _context.ProductProperties.AddAsync(productProperty, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return productProperty;
    }

    public async Task DeleteAsync(int productPropertyId, CancellationToken cancellationToken = default)
    {
        var productProperty = await _context.ProductProperties
            .FindAsync(new object[] { productPropertyId }, cancellationToken);
        if (productProperty != null)
        {
            _context.ProductProperties.Remove(productProperty);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var productProperties = await _context.ProductProperties
            .Where(pp => pp.ProductId == productId)
            .ToListAsync(cancellationToken);

        _context.ProductProperties.RemoveRange(productProperties);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

