using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfTenantRepository : ITenantRepository
{
    private readonly CafeMenuDbContext _context;

    public EfTenantRepository(CafeMenuDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .OrderBy(t => t.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tenant?> GetByIdAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.TenantId == tenantId, cancellationToken);
    }

    public async Task<Tenant?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);
    }

    public async Task<Tenant> AddAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        await _context.Tenants.AddAsync(tenant, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return tenant;
    }

    public async Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var tenant = await GetByIdAsync(tenantId, cancellationToken);
        if (tenant != null)
        {
            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

