using System.Data;

namespace PatientManagement.Infrastructure.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}