using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.SeedData;

public static class RoleSeeder
{
    public static async Task<int?> SeedSuperAdminRoleAsync(CafeMenuDbContext context, int tenantId)
    {
        var existingRole = await context.Set<Role>()
            .FirstOrDefaultAsync(r => r.Name == "SuperAdmin" && r.TenantId == tenantId && !r.IsDeleted);

        if (existingRole != null)
        {
            return existingRole.RoleId;
        }

        var role = new Role
        {
            Name = "SuperAdmin",
            TenantId = tenantId,
            IsSystem = true,
            IsActive = true,
            IsDeleted = false,
            CreatedDate = DateTime.UtcNow
        };

        await context.Set<Role>().AddAsync(role);
        await context.SaveChangesAsync();

        return role.RoleId;
    }

}

