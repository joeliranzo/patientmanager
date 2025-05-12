using Dapper;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Infrastructure.Data;

namespace PatientManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory dbConnectionFactory;

    public UserRepository(IDbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = dbConnectionFactory.CreateConnection();
        const string sql = @"
            SELECT
                id AS Id,
                email AS Email,
                password_hash AS PasswordHash,
                role AS Role,
                created_date AS CreatedDate,
                refresh_token AS RefreshToken,
                refresh_token_expiry_time AS RefreshTokenExpiryTime
            FROM users
            WHERE email = @Email";
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Email = email.ToLower() });
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        using var connection = dbConnectionFactory.CreateConnection();
        const string sql = @"
            SELECT
                id AS Id,
                email AS Email,
                password_hash AS PasswordHash,
                role AS Role,
                created_date AS CreatedDate,
                refresh_token AS RefreshToken,
                refresh_token_expiry_time AS RefreshTokenExpiryTime
            FROM users
            WHERE refresh_token = @RefreshToken";
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { RefreshToken = refreshToken });
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        using var connection = dbConnectionFactory.CreateConnection();
        const string sql = "SELECT COUNT(1) FROM users WHERE email = @Email";
        var count = await connection.ExecuteScalarAsync<int>(sql, new { Email = email.ToLower() });
        return count > 0;
    }

    public async Task<int> CreateAsync(User user)
    {
        using var connection = dbConnectionFactory.CreateConnection();
        const string sql = @"
            INSERT INTO users (email, password_hash, role, created_date)
            VALUES (@Email, @PasswordHash, @Role, @CreatedDate);
            SELECT CAST(SCOPE_IDENTITY() AS int);";
        return await connection.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        using var connection = dbConnectionFactory.CreateConnection();
        const string sql = @"
            UPDATE users
            SET refresh_token = @RefreshToken,
                refresh_token_expiry_time = @RefreshTokenExpiryTime
            WHERE id = @Id";
        await connection.ExecuteAsync(sql, new
        {
            user.RefreshToken,
            user.RefreshTokenExpiryTime,
            user.Id
        });
    }
}
