using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using CafeMenu.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfRoleRepository : GenericRepository<Role, int>, IRoleRepository
{
    public EfRoleRepository(CafeMenuDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<Role>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Include(r => r.Users)
            .Where(r => r.TenantId == tenantId && !r.IsDeleted)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Role?> GetByNameAsync(string name, int tenantId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .FirstOrDefaultAsync(r => r.Name == name && r.TenantId == tenantId && !r.IsDeleted, cancellationToken);
    }

    public async Task<bool> HasUsersAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<User>()
            .AnyAsync(u => u.RoleId == roleId && !u.IsDeleted, cancellationToken);
    }
}

