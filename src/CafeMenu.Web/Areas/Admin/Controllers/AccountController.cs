using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Models.ViewModels;
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

    public AccountController(
        IUserService userService,
        ITenantResolver tenantResolver,
        IUserRepository userRepository)
    {
        _userService = userService;
        _tenantResolver = tenantResolver;
        _userRepository = userRepository;
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

        var tenantId = _tenantResolver.GetCurrentTenantId();
        
        var user = await _userRepository.GetByUserNameAsync(model.UserName, tenantId, cancellationToken);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, $"Kullanıcı bulunamadı. Username: {model.UserName}, TenantId: {tenantId}");
            return View(model);
        }

        if (user.IsDeleted)
        {
            ModelState.AddModelError(string.Empty, "Bu kullanıcı silinmiş");
            return View(model);
        }

        var isValid = await _userService.ValidateLoginAsync(model.UserName, model.Password, tenantId, cancellationToken);

        if (!isValid)
        {
            ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre hatalı. Lütfen şifrenizi kontrol edin.");
            return View(model);
        }

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

