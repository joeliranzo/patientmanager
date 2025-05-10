using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace PatientManagement.Infrastructure.Data;

public static class DatabaseInitializer
{
    public static async Task EnsureDatabaseCreatedAsync(IConfiguration config)
    {
        var masterConn = config.GetConnectionString("DefaultConnection")!
            .Replace("Database=PatientManagementDB;", "Database=master;");
        var targetDb = "PatientManagementDB";

        using (var conn = new SqlConnection(masterConn))
        {
            await conn.OpenAsync();
            var exists = await conn.ExecuteScalarAsync<int>(
                $"SELECT COUNT(*) FROM sys.databases WHERE name = '{targetDb}'");
            if (exists == 0)
            {
                await conn.ExecuteAsync($"CREATE DATABASE [{targetDb}];");
            }
        }

        using (var conn = new SqlConnection(config.GetConnectionString("DefaultConnection")))
        {
            await conn.OpenAsync();
            var tableExists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'users'");
            if (tableExists == 0)
            {
                var sql = File.ReadAllText("Database/CreateSchema.sql");
                await conn.ExecuteAsync(sql);

                var adminHash = BCrypt.Net.BCrypt.HashPassword("123456");
                await conn.ExecuteAsync(@"
                    INSERT INTO users (email, password_hash, role, created_date)
                    VALUES (@Email, @PasswordHash, @Role, GETUTCDATE());",
                    new { Email = "admin@demo.com", PasswordHash = adminHash, Role = "Admin" });
            }
        }
    }
}