using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface IPropertyRepository
{
    Task<IReadOnlyList<Property>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<Property?> GetByIdAsync(int propertyId, int tenantId, CancellationToken cancellationToken = default);
    Task<Property> AddAsync(Property property, CancellationToken cancellationToken = default);
    Task UpdateAsync(Property property, CancellationToken cancellationToken = default);
    Task DeleteAsync(int propertyId, int tenantId, CancellationToken cancellationToken = default);
}

