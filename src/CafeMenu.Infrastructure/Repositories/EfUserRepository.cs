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
        var userNameParam = new SqlParameter("@UserName", userName);
        var plainPasswordParam = new SqlParameter("@PlainPassword", plainPassword);
        var tenantIdParam = new SqlParameter("@TenantId", tenantId);
        var isValidParam = new SqlParameter("@IsValid", System.Data.SqlDbType.Bit) { Direction = System.Data.ParameterDirection.Output };

        await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_ValidateUser @UserName, @PlainPassword, @TenantId, @IsValid OUTPUT",
            userNameParam, plainPasswordParam, tenantIdParam, isValidParam);

        return (bool)isValidParam.Value!;
    }
}

