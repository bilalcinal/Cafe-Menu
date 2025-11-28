using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfRoleRepository : IRoleRepository
{
    private readonly CafeMenuDbContext _context;

    public EfRoleRepository(CafeMenuDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByIdAsync(int roleId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.RoleId == roleId && r.TenantId == tenantId && !r.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.Users)
            .Where(r => r.TenantId == tenantId && !r.IsDeleted)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.Tenant)
            .Include(r => r.Users)
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.TenantId)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Role?> GetByNameAsync(string name, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == name && r.TenantId == tenantId && !r.IsDeleted, cancellationToken);
    }

    public async Task<int> CreateAsync(Role role, CancellationToken cancellationToken = default)
    {
        await _context.Roles.AddAsync(role, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return role.RoleId;
    }

    public async Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        _context.Roles.Update(role);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int roleId, int tenantId, CancellationToken cancellationToken = default)
    {
        var role = await GetByIdAsync(roleId, tenantId, cancellationToken);
        if (role != null && !role.IsSystem)
        {
            role.SoftDelete();
            await UpdateAsync(role, cancellationToken);
        }
    }

    public async Task<bool> HasUsersAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.RoleId == roleId && !u.IsDeleted, cancellationToken);
    }
}

