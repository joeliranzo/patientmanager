using Dapper;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Infrastructure.Data;

namespace PatientManagement.Infrastructure.Repositories;

public class UserRepository(IDbConnectionFactory dbConnectionFactory) : IUserRepository
{
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
            SELECT CAST(SCOPE_IDENTITY() as int);";

        return await connection.ExecuteScalarAsync<int>(sql, user);
    }
}