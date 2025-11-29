using CafeMenu.Application.Interfaces.Repositories.Base;
using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface IPropertyRepository : IRepository<Property, int>
{
    Task<IReadOnlyList<Property>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default);
}

