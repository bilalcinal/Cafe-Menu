namespace CafeMenu.Application.Interfaces.Services;

public interface IUserService
{
    Task<bool> ValidateLoginAsync(string userName, string password, int tenantId, CancellationToken cancellationToken = default);
    Task<int> CreateUserAsync(string name, string surname, string userName, string password, int tenantId, int roleId, CancellationToken cancellationToken = default);
}

