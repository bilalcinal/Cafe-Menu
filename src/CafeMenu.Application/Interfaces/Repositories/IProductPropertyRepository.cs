using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface IProductPropertyRepository
{
    Task<IReadOnlyList<ProductProperty>> GetByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<ProductProperty> AddAsync(ProductProperty productProperty, CancellationToken cancellationToken = default);
    Task DeleteAsync(int productPropertyId, CancellationToken cancellationToken = default);
    Task DeleteByProductIdAsync(int productId, CancellationToken cancellationToken = default);
}

