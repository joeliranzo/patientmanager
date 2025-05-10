using Dapper;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Infrastructure.Data;
using PatientManagement.Infrastructure.Security;
using System.Data;

namespace PatientManagement.Infrastructure.Repositories;

public class PatientRepository(
    IDbConnectionFactory dbConnectionFactory,
    IEncryptionService encryptionService) : IPatientRepository
{
    #region Repository Core - Functionalities
    public async Task<Patient?> GetByIdAsync(int id)
    {
        using var connection = dbConnectionFactory.CreateConnection();
        const string sql = "SELECT * FROM patients WHERE id = @Id";

        var data = await connection.QuerySingleOrDefaultAsync<PatientDataModel>(sql,
            new
            {
                Id = id
            });

        return data is null ? null : MapToDomain(data);
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        using var connection = dbConnectionFactory.CreateConnection();
        const string sql = "SELECT * FROM patients";
        var data = await connection.QueryAsync<PatientDataModel>(sql);
        return data.Select(MapToDomain);
    }

    public async Task<int> CreateAsync(Patient patient)
    {
        using var connection = dbConnectionFactory.CreateConnection();
        var data = MapToDataModel(patient);
        const string sql = @"
            INSERT INTO patients (first_name, last_name, date_of_birth, social_security_number_encrypted, address, phone_number, email, created_date, modified_date)
            VALUES (@FirstName, @LastName, @DateOfBirth, @SocialSecurityNumberEncrypted, @Address, @PhoneNumber, @Email, @CreatedDate, @ModifiedDate);
            SELECT CAST(SCOPE_IDENTITY() as int)";
        return await connection.ExecuteScalarAsync<int>(sql, data);
    }

    public async Task<bool> UpdateAsync(Patient patient)
    {
        using var connection = dbConnectionFactory.CreateConnection();
        var data = MapToDataModel(patient);
        const string sql = @"
            UPDATE patients
            SET first_name = @FirstName, last_name = @LastName, date_of_birth = @DateOfBirth,
                social_security_number_encrypted = @SocialSecurityNumberEncrypted,
                address = @Address, phone_number = @PhoneNumber, email = @Email,
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

    #endregion

    #region Mapping - Utilities

    private Patient MapToDomain(PatientDataModel model)
    {
        return new Patient
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            DateOfBirth = model.DateOfBirth,
            SocialSecurityNumber = encryptionService.DecryptToString(model.SocialSecurityNumberEncrypted),
            Address = model.Address,
            PhoneNumber = model.PhoneNumber,
            Email = model.Email,
            CreatedDate = model.CreatedDate,
            ModifiedDate = model.ModifiedDate
        };
    }

    private object MapToDataModel(Patient patient)
    {
        return new
        {
            patient.Id,
            patient.FirstName,
            patient.LastName,
            patient.DateOfBirth,
            SocialSecurityNumberEncrypted = encryptionService.EncryptToBytes(patient.SocialSecurityNumber),
            patient.Address,
            patient.PhoneNumber,
            patient.Email,
            patient.CreatedDate,
            patient.ModifiedDate
        };
    }

    private class PatientDataModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public byte[] SocialSecurityNumberEncrypted { get; set; } = default!;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    #endregion
}