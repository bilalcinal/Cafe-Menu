using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models;
using CafeMenu.Application.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Application.Services;

public class UserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserManagementService(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IRoleRepository roleRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _roleRepository = roleRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var currentTenantId = GetCurrentTenantId();
        var currentRole = GetCurrentRole();

        if (currentRole == "SuperAdmin")
        {
            var allTenants = await _tenantRepository.GetAllAsync(cancellationToken);
            var users = new List<UserDto>();
            foreach (var tenant in allTenants)
            {
                var tenantUsers = await GetAllForTenantAsync(tenant.TenantId, cancellationToken);
                users.AddRange(tenantUsers);
            }
            return users;
        }
        else
        {
            return await GetAllForTenantAsync(currentTenantId, cancellationToken);
        }
    }

    private int GetCurrentTenantId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return 1;

        var tenantIdClaim = httpContext.User.FindFirst("TenantId")?.Value;
        if (!string.IsNullOrEmpty(tenantIdClaim) && int.TryParse(tenantIdClaim, out var tenantId))
            return tenantId;

        return 1;
    }

    private string? GetCurrentRole()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return null;

        return httpContext.User.FindFirst(ClaimTypes.Role)?.Value;
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
        var user = await _userRepository
            .Query()
            .FirstOrDefaultAsync(u => u.UserId == userId && u.TenantId == tenantId, cancellationToken);
        if (user == null || user.IsDeleted)
            return null;

        var currentRole = GetCurrentRole();
        var currentTenantId = GetCurrentTenantId();

        List<Domain.Entities.Tenant> tenants;
        if (currentRole == "SuperAdmin")
        {
            tenants = (await _tenantRepository.GetAllAsync(cancellationToken)).Where(t => t.IsActive).ToList();
        }
        else
        {
            var tenant = await _tenantRepository.GetByIdAsync(currentTenantId, cancellationToken);
            tenants = tenant != null ? new List<Domain.Entities.Tenant> { tenant } : new List<Domain.Entities.Tenant>();
        }

        var roles = await _roleRepository.GetAllForTenantAsync(user.TenantId, cancellationToken);

        return new UserViewModel
        {
            UserId = user.UserId,
            Name = user.Name,
            Surname = user.Surname,
            UserName = user.UserName,
            TenantId = user.TenantId,
            RoleId = user.RoleId,
            AvailableTenants = tenants.Select(t => new TenantDto
            {
                TenantId = t.TenantId,
                Name = t.Name,
                Code = t.Code,
                IsActive = t.IsActive,
                CreatedDate = t.CreatedDate
            }).ToList(),
            AvailableRoles = roles.Where(r => {
                if (r.IsSystem && r.Name == "SuperAdmin")
                    return user.TenantId == 1 && currentRole == "SuperAdmin";
                return true;
            }).Select(r => new RoleDto
            {
                RoleId = r.RoleId,
                Name = r.Name,
                TenantId = r.TenantId,
                TenantName = string.Empty,
                IsSystem = r.IsSystem,
                IsActive = r.IsActive,
                CreatedDate = r.CreatedDate,
                UserCount = 0
            }).ToList()
        };
    }

    public async Task<int> CreateAsync(UserViewModel viewModel, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(viewModel.Password))
            throw new InvalidOperationException("Şifre gereklidir");

        var currentRole = GetCurrentRole();
        var currentTenantId = GetCurrentTenantId();

        int tenantId = viewModel.TenantId;
        if (currentRole != "SuperAdmin")
        {
            tenantId = currentTenantId;
        }

        if (viewModel.RoleId <= 0)
            throw new InvalidOperationException("Rol seçimi gereklidir");

        var role = await _roleRepository
            .Query()
            .FirstOrDefaultAsync(r => r.RoleId == viewModel.RoleId && r.TenantId == tenantId && !r.IsDeleted, cancellationToken);
        if (role == null)
            throw new InvalidOperationException("Geçersiz rol");

        if (currentRole != "SuperAdmin" && role.Name == "SuperAdmin")
            throw new InvalidOperationException("SuperAdmin rolü sadece SuperAdmin tarafından atanabilir");

        return await _userRepository.CreateUserWithHashAsync(
            viewModel.Name,
            viewModel.Surname,
            viewModel.UserName,
            viewModel.Password,
            tenantId,
            viewModel.RoleId,
            cancellationToken);
    }

    public async Task UpdateAsync(UserViewModel viewModel, CancellationToken cancellationToken = default)
    {
        var currentRole = GetCurrentRole();
        var currentTenantId = GetCurrentTenantId();

        var user = await _userRepository
            .Query()
            .FirstOrDefaultAsync(u => u.UserId == viewModel.UserId && u.TenantId == viewModel.TenantId, cancellationToken);
        if (user == null || user.IsDeleted)
            throw new InvalidOperationException("Kullanıcı bulunamadı");

        if (currentRole != "SuperAdmin" && user.TenantId != currentTenantId)
            throw new InvalidOperationException("Bu kullanıcıyı düzenleme yetkiniz yok");

        if (currentRole != "SuperAdmin")
        {
            viewModel.TenantId = currentTenantId;
        }

        if (viewModel.RoleId > 0)
        {
            var role = await _roleRepository
                .Query()
                .FirstOrDefaultAsync(r => r.RoleId == viewModel.RoleId && r.TenantId == viewModel.TenantId && !r.IsDeleted, cancellationToken);
            if (role == null)
                throw new InvalidOperationException("Geçersiz rol");

            if (currentRole != "SuperAdmin" && role.Name == "SuperAdmin")
                throw new InvalidOperationException("SuperAdmin rolü sadece SuperAdmin tarafından atanabilir");

            user.RoleId = viewModel.RoleId;
        }

        user.Name = viewModel.Name;
        user.Surname = viewModel.Surname;
        user.UserName = viewModel.UserName;
        user.TenantId = viewModel.TenantId;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(viewModel.Password))
        {
            await _userRepository.UpdatePasswordAsync(user.UserId, viewModel.Password, viewModel.TenantId, cancellationToken);
        }
    }

    public async Task DeleteAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository
            .Query()
            .FirstOrDefaultAsync(u => u.UserId == userId && u.TenantId == tenantId, cancellationToken);
        if (user != null)
        {
            user.SoftDelete();
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);
        }
    }
}

