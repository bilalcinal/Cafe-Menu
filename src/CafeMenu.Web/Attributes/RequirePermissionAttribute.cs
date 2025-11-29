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
            var routeData = context.RouteData;
            var area = routeData.Values["area"]?.ToString();
            
            if (area == "Customer")
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "Customer" });
            }
            else if (area == "Admin")
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "Admin" });
            }
            else
            {
                context.Result = new RedirectToActionResult("Login", "CustomerAccount", null);
            }
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
            var routeData = context.RouteData;
            var area = routeData.Values["area"]?.ToString();
            
            if (area == "Customer")
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", new { area = "Customer" });
            }
            else if (area == "Admin")
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", new { area = "Admin" });
            }
            else
            {
                context.Result = new RedirectToActionResult("AccessDenied", "CustomerAccount", null);
            }
        }
    }
}

