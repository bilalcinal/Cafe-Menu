using CafeMenu.Application.Interfaces.Repositories.Base;
using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface ITenantRepository : IRepository<Tenant, int>
{
    Task<Tenant?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}

