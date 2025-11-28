using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.Repositories;

public class EfPropertyRepository : IPropertyRepository
{
    private readonly CafeMenuDbContext _context;

    public EfPropertyRepository(CafeMenuDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Property>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .Where(p => p.TenantId == tenantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Property?> GetByIdAsync(int propertyId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Properties
            .FirstOrDefaultAsync(p => p.PropertyId == propertyId && p.TenantId == tenantId, cancellationToken);
    }

    public async Task<Property> AddAsync(Property property, CancellationToken cancellationToken = default)
    {
        await _context.Properties.AddAsync(property, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return property;
    }

    public async Task UpdateAsync(Property property, CancellationToken cancellationToken = default)
    {
        _context.Properties.Update(property);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int propertyId, int tenantId, CancellationToken cancellationToken = default)
    {
        var property = await GetByIdAsync(propertyId, tenantId, cancellationToken);
        if (property != null)
        {
            _context.Properties.Remove(property);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

