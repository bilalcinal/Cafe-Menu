using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using CafeMenu.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfTenantRepository : GenericRepository<Tenant, int>, ITenantRepository
{
    public EfTenantRepository(CafeMenuDbContext context)
        : base(context)
    {
    }

    public override async Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query()
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tenant?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await Query()
            .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);
    }
}

