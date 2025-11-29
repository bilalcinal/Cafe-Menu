using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using CafeMenu.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfProductPropertyRepository : GenericRepository<ProductProperty, int>, IProductPropertyRepository
{
    public EfProductPropertyRepository(CafeMenuDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<ProductProperty>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Include(pp => pp.Property)
            .Where(pp => pp.ProductId == productId)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var productProperties = await Query()
            .Where(pp => pp.ProductId == productId)
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(productProperties);
    }
}

