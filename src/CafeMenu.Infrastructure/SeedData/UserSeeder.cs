using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CafeMenu.Infrastructure.SeedData;

public static class UserSeeder
{
    public static async Task SeedAdminUserAsync(CafeMenuDbContext context, int tenantId, int roleId)
    {
        var hasAnyUser = await context.Set<User>()
            .AnyAsync(u => !u.IsDeleted);

        if (hasAnyUser)
        {
            return;
        }

        var connection = context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        using var command = connection.CreateCommand();
        command.CommandText = @"
            DECLARE @Salt VARBINARY(32) = CRYPT_GEN_RANDOM(32);
            DECLARE @Hash VARBINARY(64) = HASHBYTES('SHA2_256', @Salt + CAST(@PlainPassword AS VARBINARY(200)));
            INSERT INTO [USER] (NAME, SURNAME, USERNAME, HASHPASSWORD, SALTPASSWORD, TENANTID, ROLEID, IsDeleted, CreatedDate, CreatorUserId)
            VALUES (@Name, @Surname, @UserName, @Hash, @Salt, @TenantId, @RoleId, 0, GETUTCDATE(), NULL)";
        command.CommandType = System.Data.CommandType.Text;

        var nameParam = new SqlParameter("@Name", System.Data.SqlDbType.NVarChar, 100) { Value = "Admin" };
        var surnameParam = new SqlParameter("@Surname", System.Data.SqlDbType.NVarChar, 100) { Value = "User" };
        var userNameParam = new SqlParameter("@UserName", System.Data.SqlDbType.NVarChar, 100) { Value = "admin" };
        var plainPasswordParam = new SqlParameter("@PlainPassword", System.Data.SqlDbType.NVarChar, 200) { Value = "Admin123!" };
        var tenantIdParam = new SqlParameter("@TenantId", System.Data.SqlDbType.Int) { Value = tenantId };
        var roleIdParam = new SqlParameter("@RoleId", System.Data.SqlDbType.Int) { Value = roleId };

        command.Parameters.Add(nameParam);
        command.Parameters.Add(surnameParam);
        command.Parameters.Add(userNameParam);
        command.Parameters.Add(plainPasswordParam);
        command.Parameters.Add(tenantIdParam);
        command.Parameters.Add(roleIdParam);

        await command.ExecuteNonQueryAsync();
    }
}

