using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfPermissionRepository : IPermissionRepository
{
    private readonly CafeMenuDbContext _context;

    public EfPermissionRepository(CafeMenuDbContext context)
    {
        _context = context;
    }

    public async Task<Permission?> GetByIdAsync(int permissionId, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(p => p.PermissionId == permissionId, cancellationToken);
    }

    public async Task<Permission?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(p => p.Key == key, cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .OrderBy(p => p.GroupName)
            .ThenBy(p => p.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Permission>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Permissions
            .Where(p => p.IsActive)
            .OrderBy(p => p.GroupName)
            .ThenBy(p => p.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CreateAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        await _context.Permissions.AddAsync(permission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return permission.PermissionId;
    }

    public async Task UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
    {
        _context.Permissions.Update(permission);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

