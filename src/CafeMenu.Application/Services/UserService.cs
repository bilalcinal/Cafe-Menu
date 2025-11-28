using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;

namespace CafeMenu.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> ValidateLoginAsync(string userName, string password, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _userRepository.ValidateUserPasswordAsync(userName, password, tenantId, cancellationToken);
    }

    public async Task<int> CreateUserAsync(string name, string surname, string userName, string password, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _userRepository.CreateUserWithHashAsync(name, surname, userName, password, tenantId, cancellationToken);
    }
}

