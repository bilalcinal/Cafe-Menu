using CafeMenu.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CafeMenu.Infrastructure.Services;

public class TenantResolver : ITenantResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TenantResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetCurrentTenantId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return 1;

        if (httpContext.User?.Identity?.IsAuthenticated == true)
        {
            var tenantIdClaim = httpContext.User.FindFirst("TenantId")?.Value;
            if (!string.IsNullOrEmpty(tenantIdClaim) && int.TryParse(tenantIdClaim, out var tenantIdFromClaim))
            {
                return tenantIdFromClaim;
            }
        }

        if (httpContext.Session != null)
        {
            var sessionTenantId = httpContext.Session.GetString("TenantId");
            if (!string.IsNullOrEmpty(sessionTenantId) && int.TryParse(sessionTenantId, out var tenantIdFromSession))
            {
                return tenantIdFromSession;
            }
        }

        var queryString = httpContext.Request.Query["tenantId"].ToString();
        if (!string.IsNullOrEmpty(queryString) && int.TryParse(queryString, out var tenantIdFromQuery))
        {
            if (httpContext.Session != null)
            {
                httpContext.Session.SetString("TenantId", tenantIdFromQuery.ToString());
            }
            return tenantIdFromQuery;
        }

        var host = httpContext.Request.Host.Host;
        if (!string.IsNullOrEmpty(host) && host.Contains('.'))
        {
            var subdomain = host.Split('.')[0];
            if (int.TryParse(subdomain, out var tenantIdFromSubdomain))
            {
                if (httpContext.Session != null)
                {
                    httpContext.Session.SetString("TenantId", tenantIdFromSubdomain.ToString());
                }
                return tenantIdFromSubdomain;
            }
        }

        return 1;
    }
}

