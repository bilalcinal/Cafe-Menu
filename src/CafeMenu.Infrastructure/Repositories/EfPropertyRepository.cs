using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using CafeMenu.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfPropertyRepository : GenericRepository<Property, int>, IPropertyRepository
{
    public EfPropertyRepository(CafeMenuDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Property>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(p => p.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }
}

