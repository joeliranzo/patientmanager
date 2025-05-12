namespace PatientManagement.Infrastructure.Repositories;

using Dapper;
using PatientManagement.Application.DTOs.Patient;
using PatientManagement.Application.DTOs.Shared;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Infrastructure.Data;
using PatientManagement.Infrastructure.Mappings;
using PatientManagement.Infrastructure.Security;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

public class PatientRepository(
    IDbConnectionFactory dbConnectionFactory,
    IEncryptionService encryptionService) : IPatientRepository
{
    public async Task<Patient?> GetByIdAsync(int id)
    {
        using var connection = dbConnectionFactory.CreateConnection();
        const string sql = "SELECT * FROM patients WHERE id = @Id";
        var model = await connection.QuerySingleOrDefaultAsync<PatientDataModel>(sql, new { Id = id });
        return model?.ToDomain(encryptionService);
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        using var connection = dbConnectionFactory.CreateConnection();
        const string sql = "SELECT * FROM patients";
        var models = await connection.QueryAsync<PatientDataModel>(sql);
        return models.Select(m => m.ToDomain(encryptionService));
    }

    public async Task<int> CreateAsync(Patient patient)
    {
        using var connection = dbConnectionFactory.CreateConnection();
        var data = patient.ToDataModel(encryptionService);
        const string sql = @"
            INSERT INTO patients (
                first_name,
                last_name,
                date_of_birth,
                social_security_number_encrypted,
                address,
                phone_number,
                email,
                created_date,
                modified_date
            )
            VALUES (
                @FirstName,
                @LastName,
                @DateOfBirth,
                @SocialSecurityNumberEncrypted,
                @Address,
                @PhoneNumber,
                @Email,
                @CreatedDate,
                @ModifiedDate
            );
            SELECT CAST(SCOPE_IDENTITY() as int)";
        return await connection.ExecuteScalarAsync<int>(sql, data);
    }

    public async Task<bool> UpdateAsync(Patient patient)
    {
        using var connection = dbConnectionFactory.CreateConnection();
        var data = patient.ToDataModel(encryptionService);
        const string sql = @"
            UPDATE patients
            SET
                first_name = @FirstName,
                last_name = @LastName,
                date_of_birth = @DateOfBirth,
                social_security_number_encrypted = @SocialSecurityNumberEncrypted,
                address = @Address,
                phone_number = @PhoneNumber,
                email = @Email,
                modified_date = @ModifiedDate
            WHERE id = @Id";
        return await connection.ExecuteAsync(sql, data) > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = dbConnectionFactory.CreateConnection();
        const string sql = "DELETE FROM patients WHERE id = @Id";
        return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
    }

    public async Task<PagedResult<Patient>> QueryAsync(PatientQueryParametersDto parameters)
    {
        using var connection = dbConnectionFactory.CreateConnection();

        var filters = new List<string>();
        var sqlParams = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(parameters.FirstName))
        {
            filters.Add("first_name LIKE @FirstName");
            sqlParams.Add("FirstName", $"%{parameters.FirstName}%");
        }

        if (!string.IsNullOrWhiteSpace(parameters.LastName))
        {
            filters.Add("last_name LIKE @LastName");
            sqlParams.Add("LastName", $"%{parameters.LastName}%");
        }

        if (!string.IsNullOrWhiteSpace(parameters.Email))
        {
            filters.Add("email LIKE @Email");
            sqlParams.Add("Email", $"%{parameters.Email}%");
        }

        var whereClause = filters.Count != 0 ? "WHERE " + string.Join(" AND ", filters) : "";

        var sortBy = parameters.SortBy?.ToLower() switch
        {
            "first_name" or "last_name" or "email" or "created_date" or "modified_date" => parameters.SortBy,
            _ => "created_date"
        };

        var sortOrder = parameters.SortOrder?.ToLower() == "asc" ? "ASC" : "DESC";
        var offset = (parameters.Page - 1) * parameters.PageSize;

        var countSql = $"SELECT COUNT(*) FROM patients {whereClause}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, sqlParams);

        var dataSql = $@"
            SELECT 
                id, first_name, last_name, date_of_birth, social_security_number_encrypted,
                address, phone_number, email, created_date, modified_date
            FROM patients
            {whereClause}
            ORDER BY {sortBy} {sortOrder}
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        sqlParams.Add("Offset", offset);
        sqlParams.Add("PageSize", parameters.PageSize);

        var data = await connection.QueryAsync<PatientDataModel>(dataSql, sqlParams);
        var patients = data.Select(p => p.ToDomain(encryptionService));

        return new PagedResult<Patient>
        {
            Items = patients,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }
}
