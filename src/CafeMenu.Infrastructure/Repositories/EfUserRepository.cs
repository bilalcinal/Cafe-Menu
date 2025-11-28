using CafeMenu.Application.Interfaces.Repositories;
using CafeMenu.Domain.Entities;
using CafeMenu.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CafeMenu.Infrastructure.Repositories;

public class EfUserRepository : IUserRepository
{
    private readonly CafeMenuDbContext _context;

    public EfUserRepository(CafeMenuDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUserNameAsync(string userName, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == userName && u.TenantId == tenantId, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId && u.TenantId == tenantId, cancellationToken);
    }

    public async Task<int> CreateUserWithHashAsync(string name, string surname, string userName, string plainPassword, int tenantId, CancellationToken cancellationToken = default)
    {
        var nameParam = new SqlParameter("@Name", name);
        var surnameParam = new SqlParameter("@Surname", surname);
        var userNameParam = new SqlParameter("@UserName", userName);
        var plainPasswordParam = new SqlParameter("@PlainPassword", plainPassword);
        var tenantIdParam = new SqlParameter("@TenantId", tenantId);
        var userIdParam = new SqlParameter("@UserId", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output };

        await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_CreateUser @Name, @Surname, @UserName, @PlainPassword, @TenantId, @UserId OUTPUT",
            nameParam, surnameParam, userNameParam, plainPasswordParam, tenantIdParam, userIdParam);

        return (int)userIdParam.Value!;
    }

    public async Task<bool> ValidateUserPasswordAsync(string userName, string plainPassword, int tenantId, CancellationToken cancellationToken = default)
    {
        var connection = _context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        using var command = connection.CreateCommand();
        command.CommandText = "EXEC sp_ValidateUser @UserName, @PlainPassword, @TenantId, @IsValid OUTPUT";
        command.CommandType = System.Data.CommandType.Text;

        var userNameParam = new SqlParameter("@UserName", SqlDbType.NVarChar, 100) { Value = userName };
        var plainPasswordParam = new SqlParameter("@PlainPassword", SqlDbType.NVarChar, 200) { Value = plainPassword };
        var tenantIdParam = new SqlParameter("@TenantId", SqlDbType.Int) { Value = tenantId };
        var isValidParam = new SqlParameter("@IsValid", SqlDbType.Bit) { Direction = ParameterDirection.Output };

        command.Parameters.Add(userNameParam);
        command.Parameters.Add(plainPasswordParam);
        command.Parameters.Add(tenantIdParam);
        command.Parameters.Add(isValidParam);

        await command.ExecuteNonQueryAsync(cancellationToken);

        if (isValidParam.Value == DBNull.Value)
            return false;

        return Convert.ToBoolean(isValidParam.Value);
    }

    public async Task<IReadOnlyList<User>> GetAllForTenantAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.TenantId == tenantId && !u.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdatePasswordAsync(int userId, string plainPassword, int tenantId, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, tenantId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException("Kullanıcı bulunamadı");

        var connection = _context.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        using var command = connection.CreateCommand();
        command.CommandText = @"
            DECLARE @Salt VARBINARY(32) = CRYPT_GEN_RANDOM(32);
            DECLARE @Hash VARBINARY(64) = HASHBYTES('SHA2_256', @Salt + CAST(@PlainPassword AS VARBINARY(200)));
            UPDATE [USER] SET HASHPASSWORD = @Hash, SALTPASSWORD = @Salt WHERE USERID = @UserId AND TENANTID = @TenantId";
        command.CommandType = System.Data.CommandType.Text;

        var userIdParam = new SqlParameter("@UserId", SqlDbType.Int) { Value = userId };
        var tenantIdParam = new SqlParameter("@TenantId", SqlDbType.Int) { Value = tenantId };
        var plainPasswordParam = new SqlParameter("@PlainPassword", SqlDbType.NVarChar, 200) { Value = plainPassword };

        command.Parameters.Add(userIdParam);
        command.Parameters.Add(tenantIdParam);
        command.Parameters.Add(plainPasswordParam);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}

