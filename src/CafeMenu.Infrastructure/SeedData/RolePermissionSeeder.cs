using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.SeedData;

public static class RolePermissionSeeder
{
    public static async Task SeedSuperAdminPermissionsAsync(CafeMenuDbContext context, int roleId)
    {
        var allPermissions = await context.Set<Permission>()
            .Where(p => p.IsActive)
            .ToListAsync();

        var existingRolePermissions = await context.Set<RolePermission>()
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        foreach (var permission in allPermissions)
        {
            if (!existingRolePermissions.Contains(permission.PermissionId))
            {
                var rolePermission = new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission.PermissionId
                };

                await context.Set<RolePermission>().AddAsync(rolePermission);
            }
        }

        await context.SaveChangesAsync();
    }

    public static async Task SeedTenantAdminPermissionsAsync(CafeMenuDbContext context, int roleId)
    {
        var allPermissions = await context.Set<Permission>()
            .Where(p => p.IsActive && !p.Key.StartsWith("Admin.Tenant."))
            .ToListAsync();

        var existingRolePermissions = await context.Set<RolePermission>()
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        foreach (var permission in allPermissions)
        {
            if (!existingRolePermissions.Contains(permission.PermissionId))
            {
                var rolePermission = new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission.PermissionId
                };

                await context.Set<RolePermission>().AddAsync(rolePermission);
            }
        }

        await context.SaveChangesAsync();
    }
}

