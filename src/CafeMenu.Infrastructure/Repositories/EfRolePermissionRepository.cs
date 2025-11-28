using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfRolePermissionRepository : IRolePermissionRepository
{
    private readonly CafeMenuDbContext _context;

    public EfRolePermissionRepository(CafeMenuDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<RolePermission>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await _context.RolePermissions
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RolePermission rolePermission, CancellationToken cancellationToken = default)
    {
        await _context.RolePermissions.AddAsync(rolePermission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
    {
        var rolePermission = await _context.RolePermissions
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId, cancellationToken);
        if (rolePermission != null)
        {
            _context.RolePermissions.Remove(rolePermission);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task RemoveAllForRoleAsync(int roleId, CancellationToken cancellationToken = default)
    {
        var rolePermissions = await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);
        _context.RolePermissions.RemoveRange(rolePermissions);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

