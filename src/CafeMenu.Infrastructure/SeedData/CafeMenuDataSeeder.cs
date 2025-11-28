using CafeMenu.Infrastructure.Persistence;

namespace CafeMenu.Infrastructure.SeedData;

public static class CafeMenuDataSeeder
{
    public static async Task SeedAsync(CafeMenuDbContext context)
    {
        await PermissionSeeder.SeedAsync(context);

        var adminTenantId = await TenantSeeder.SeedAdminTenantAsync(context);
        if (adminTenantId == null)
        {
            return;
        }

        var superAdminRoleId = await RoleSeeder.SeedSuperAdminRoleAsync(context, adminTenantId.Value);
        if (superAdminRoleId == null)
        {
            return;
        }

        await UserSeeder.SeedAdminUserAsync(context, adminTenantId.Value, superAdminRoleId.Value);

        await RolePermissionSeeder.SeedSuperAdminPermissionsAsync(context, superAdminRoleId.Value);

    }
}

