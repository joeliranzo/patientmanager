namespace PatientManagement.Infrastructure.Mappings;

using PatientManagement.Domain.Entities;
using PatientManagement.Infrastructure.Security;
using System;

public class PatientDataModel
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

public static class PatientDataModelMappings
{
    public static Patient ToDomain(this PatientDataModel model, IEncryptionService encryptionService)
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

    public static object ToDataModel(this Patient patient, IEncryptionService encryptionService) => new
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
