using CafeMenu.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;

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

        var queryString = httpContext.Request.Query["tenantId"].ToString();
        if (!string.IsNullOrEmpty(queryString) && int.TryParse(queryString, out var tenantId))
        {
            return tenantId;
        }

        return 1;
    }
}

