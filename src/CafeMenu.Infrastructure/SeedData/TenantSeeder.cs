using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.SeedData;

public static class TenantSeeder
{
    public static async Task<int?> SeedAdminTenantAsync(CafeMenuDbContext context)
    {
        var hasAnyTenant = await context.Set<Tenant>().AnyAsync();
        if (hasAnyTenant)
        {
            var adminTenant = await context.Set<Tenant>()
                .OrderBy(t => t.TenantId)
                .FirstOrDefaultAsync();
            return adminTenant?.TenantId;
        }

        var connection = context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        using var command = connection.CreateCommand();
        command.CommandText = @"
            SET IDENTITY_INSERT [TENANT] ON;
            INSERT INTO [TENANT] (TENANTID, NAME, CODE, ISACTIVE, CREATEDDATE)
            VALUES (1, @Name, @Code, @IsActive, @CreatedDate);
            SET IDENTITY_INSERT [TENANT] OFF;";
        command.CommandType = System.Data.CommandType.Text;

        var nameParam = new SqlParameter("@Name", System.Data.SqlDbType.NVarChar, 200) { Value = "Admin Tenant" };
        var codeParam = new SqlParameter("@Code", System.Data.SqlDbType.NVarChar, 50) { Value = "ADMIN" };
        var isActiveParam = new SqlParameter("@IsActive", System.Data.SqlDbType.Bit) { Value = true };
        var createdDateParam = new SqlParameter("@CreatedDate", System.Data.SqlDbType.DateTime2) { Value = DateTime.UtcNow };

        command.Parameters.Add(nameParam);
        command.Parameters.Add(codeParam);
        command.Parameters.Add(isActiveParam);
        command.Parameters.Add(createdDateParam);

        try
        {
            await command.ExecuteNonQueryAsync();
            return 1;
        }
        catch
        {
            var tenant = await context.Set<Tenant>()
                .OrderBy(t => t.TenantId)
                .FirstOrDefaultAsync();
            return tenant?.TenantId;
        }
    }
}

