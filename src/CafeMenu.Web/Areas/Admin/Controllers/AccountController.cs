using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CafeMenu.Web.Areas.Admin.Controllers;

[Area("Admin")]
[AllowAnonymous]
public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly ITenantResolver _tenantResolver;
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IPermissionService _permissionService;

    public AccountController(
        IUserService userService,
        ITenantResolver tenantResolver,
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IRoleRepository roleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IPermissionRepository permissionRepository,
        IPermissionService permissionService)
    {
        _userService = userService;
        _tenantResolver = tenantResolver;
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _roleRepository = roleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _permissionRepository = permissionRepository;
        _permissionService = permissionService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        var allTenants = await _tenantRepository.GetAllAsync(cancellationToken);
        User? user = null;
        int? foundTenantId = null;

        foreach (var tenant in allTenants)
        {
            var testUser = await _userRepository.GetByUserNameAsync(model.UserName, tenant.TenantId, cancellationToken);
            if (testUser != null && !testUser.IsDeleted)
            {
                var isValid = await _userService.ValidateLoginAsync(model.UserName, model.Password, tenant.TenantId, cancellationToken);
                if (isValid)
                {
                    user = testUser;
                    foundTenantId = tenant.TenantId;
                    break;
                }
            }
        }

        if (user == null || foundTenantId == null)
        {
            ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı.");
            return View(model);
        }

        var tenantId = foundTenantId.Value;

        var role = await _roleRepository
            .Query()
            .FirstOrDefaultAsync(r => r.RoleId == user.RoleId && r.TenantId == tenantId && !r.IsDeleted, cancellationToken);
        var roleName = role?.Name ?? string.Empty;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Role, roleName),
            new Claim("TenantId", tenantId.ToString()),
            new Claim("RoleId", user.RoleId.ToString())
        };

        var rolePermissions = await _rolePermissionRepository.GetByRoleIdAsync(user.RoleId, cancellationToken);
        var permissionKeys = new List<string>();

        if (roleName == "SuperAdmin")
        {
            var allPermissions = await _permissionRepository.GetActiveAsync(cancellationToken);
            permissionKeys = allPermissions.Select(p => p.Key).ToList();
        }
        else
        {
            permissionKeys = rolePermissions
                .Where(rp => rp.Permission != null)
                .Select(rp => rp.Permission!.Key)
                .Where(k => !string.IsNullOrEmpty(k))
                .ToList();
        }

        foreach (var permissionKey in permissionKeys)
        {
            claims.Add(new Claim("Permission", permissionKey));
        }

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}

