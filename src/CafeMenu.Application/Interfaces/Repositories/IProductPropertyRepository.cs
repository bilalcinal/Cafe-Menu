using CafeMenu.Application.Interfaces.Repositories.Base;
using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface IProductPropertyRepository : IRepository<ProductProperty, int>
{
    Task<IReadOnlyList<ProductProperty>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task DeleteByProductIdAsync(int productId, CancellationToken cancellationToken = default);
}

