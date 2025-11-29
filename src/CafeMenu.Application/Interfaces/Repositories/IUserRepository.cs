using CafeMenu.Application.Interfaces.Repositories.Base;
using CafeMenu.Domain.Entities;

namespace CafeMenu.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User, int>
{
    Task<User?> GetByUserNameAsync(string userName, int tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<int> CreateUserWithHashAsync(string name, string surname, string userName, string plainPassword, int tenantId, int roleId, CancellationToken cancellationToken = default);
    Task<bool> ValidateUserPasswordAsync(string userName, string plainPassword, int tenantId, CancellationToken cancellationToken = default);
    Task UpdatePasswordAsync(int userId, string plainPassword, int tenantId, CancellationToken cancellationToken = default);
}
