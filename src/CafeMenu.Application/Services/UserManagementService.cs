using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models;
using CafeMenu.Application.Models.ViewModels;

namespace CafeMenu.Application.Services;

public class UserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserService _userService;

    public UserManagementService(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IUserService userService)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _userService = userService;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var allTenants = await _tenantRepository.GetAllAsync(cancellationToken);
        var tenantMap = allTenants.ToDictionary(t => t.TenantId, t => t.Name);

        var users = new List<UserDto>();
        foreach (var tenant in allTenants)
        {
            var tenantUsers = await GetAllForTenantAsync(tenant.TenantId, cancellationToken);
            users.AddRange(tenantUsers);
        }

        return users;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllForTenantAsync(tenantId, cancellationToken);
        var tenant = await _tenantRepository.GetByIdAsync(tenantId, cancellationToken);
        var tenantName = tenant?.Name ?? string.Empty;

        return users.Select(u => new UserDto
        {
            UserId = u.UserId,
            Name = u.Name,
            Surname = u.Surname,
            UserName = u.UserName,
            TenantId = u.TenantId,
            TenantName = tenantName
        }).ToList();
    }

    public async Task<UserViewModel?> GetByIdAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, tenantId, cancellationToken);
        if (user == null || user.IsDeleted)
            return null;

        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);

        return new UserViewModel
        {
            UserId = user.UserId,
            Name = user.Name,
            Surname = user.Surname,
            UserName = user.UserName,
            TenantId = user.TenantId,
            AvailableTenants = tenants.Select(t => new TenantDto
            {
                TenantId = t.TenantId,
                Name = t.Name,
                Code = t.Code,
                IsActive = t.IsActive,
                CreatedDate = t.CreatedDate
            }).ToList()
        };
    }

    public async Task<int> CreateAsync(UserViewModel viewModel, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(viewModel.Password))
            throw new InvalidOperationException("Şifre gereklidir");

        return await _userService.CreateUserAsync(
            viewModel.Name,
            viewModel.Surname,
            viewModel.UserName,
            viewModel.Password,
            viewModel.TenantId,
            cancellationToken);
    }

    public async Task UpdateAsync(UserViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(viewModel.UserId, viewModel.TenantId, cancellationToken);
        if (user == null || user.IsDeleted)
            throw new InvalidOperationException("Kullanıcı bulunamadı");

        user.Name = viewModel.Name;
        user.Surname = viewModel.Surname;
        user.UserName = viewModel.UserName;
        user.TenantId = viewModel.TenantId;

        await _userRepository.UpdateAsync(user, cancellationToken);

        if (!string.IsNullOrWhiteSpace(viewModel.Password))
        {
            await _userRepository.UpdatePasswordAsync(user.UserId, viewModel.Password, viewModel.TenantId, cancellationToken);
        }
    }

    public async Task DeleteAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, tenantId, cancellationToken);
        if (user != null)
        {
            user.SoftDelete();
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
    }
}

