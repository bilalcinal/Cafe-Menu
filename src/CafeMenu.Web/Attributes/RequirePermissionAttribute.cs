using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace CafeMenu.Web.Attributes;

public class RequirePermissionAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _permissionKey;

    public RequirePermissionAttribute(string permissionKey)
    {
        _permissionKey = permissionKey;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Result = new RedirectToActionResult("Login", "Account", new { area = "Admin" });
            return;
        }

        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        if (role == "SuperAdmin")
        {
            return;
        }

        var permissions = user.FindAll("Permission").Select(c => c.Value).ToList();
        if (!permissions.Contains(_permissionKey))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", new { area = "Admin" });
        }
    }
}

