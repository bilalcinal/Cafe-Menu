using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Application.Interfaces.Services;
using CafeMenu.Application.Services;
using CafeMenu.Infrastructure.Repositories;
using CafeMenu.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CafeMenu.Infrastructure.IoC;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddMemoryCache();

        services.AddHttpClient();
        services.AddHttpClient<ICurrencyService, CurrencyService>();

        services.AddHttpContextAccessor();

        services.AddScoped<ICategoryRepository, EfCategoryRepository>();
        services.AddScoped<IProductRepository, EfProductRepository>();
        services.AddScoped<IPropertyRepository, EfPropertyRepository>();
        services.AddScoped<IProductPropertyRepository, EfProductPropertyRepository>();
        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<ITenantRepository, EfTenantRepository>();
        services.AddScoped<IRoleRepository, EfRoleRepository>();
        services.AddScoped<IPermissionRepository, EfPermissionRepository>();
        services.AddScoped<IRolePermissionRepository, EfRolePermissionRepository>();

        services.AddScoped<IProductCacheService, ProductCacheService>();
        services.AddScoped<ITenantResolver, TenantResolver>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<IMenuPdfService, MenuPdfService>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IPermissionService, PermissionService>();

        services.AddScoped<CustomerMenuService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<ProductService>();
        services.AddScoped<PropertyService>();
        services.AddScoped<DashboardService>();
        services.AddScoped<TenantService>();
        services.AddScoped<UserManagementService>();
        services.AddScoped<RoleManagementService>();

        return services;
    }
}

