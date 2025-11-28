using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default);
    Task<Category> AddAsync(Category category, CancellationToken cancellationToken = default);
    Task UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task DeleteAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int categoryId, int tenantId, CancellationToken cancellationToken = default);
}

