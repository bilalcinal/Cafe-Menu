using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using CafeMenu.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfPermissionRepository : GenericRepository<Permission, int>, IPermissionRepository
{
    public EfPermissionRepository(CafeMenuDbContext context)
        : base(context)
    {
    }

    public async Task<Permission?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await Query()
            .FirstOrDefaultAsync(p => p.Key == key, cancellationToken);
    }

    public override async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query()
            .OrderBy(p => p.GroupName)
            .ThenBy(p => p.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await Query()
            .Where(p => p.IsActive)
            .OrderBy(p => p.GroupName)
            .ThenBy(p => p.Key)
            .ToListAsync(cancellationToken);
    }
}

