using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using CafeMenu.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfRolePermissionRepository : GenericRepository<RolePermission, int>, IRolePermissionRepository
{
    public EfRolePermissionRepository(CafeMenuDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<RolePermission>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task RemoveAllForRoleAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var rolePermissions = await Query()
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);
        _dbSet.RemoveRange(rolePermissions);
    }
}

