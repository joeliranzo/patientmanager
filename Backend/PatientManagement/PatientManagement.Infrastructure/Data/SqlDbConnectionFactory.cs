using Microsoft.Data.SqlClient;
using System.Data;

namespace PatientManagement.Infrastructure.Data;

public class SqlDbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}