using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models.ViewModels;
using CafeMenu.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public AccountController(
        IUserService userService,
        ITenantResolver tenantResolver,
        IUserRepository userRepository,
        ITenantRepository tenantRepository)
    {
        _userService = userService;
        _tenantResolver = tenantResolver;
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
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

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("TenantId", tenantId.ToString())
        };

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

