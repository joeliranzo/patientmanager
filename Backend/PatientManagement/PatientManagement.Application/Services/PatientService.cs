using PatientManagement.Application.DTOs.Patient;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Services;

public class PatientService(IPatientRepository repository) : IPatientService
{
    public async Task<IEnumerable<PatientResponseDto>> GetAllAsync()
    {
        var patients = await repository.GetAllAsync();
        return patients.Select(MapToResponse);
    }

    public async Task<PatientResponseDto?> GetByIdAsync(int id)
    {
        var patient = await repository.GetByIdAsync(id);
        return patient is null ? null : MapToResponse(patient);
    }

    public async Task<int> CreateAsync(CreatePatientRequestDto dto)
    {
        var patient = new Patient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            SocialSecurityNumber = dto.SocialSecurityNumber,
            Address = dto.Address,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            CreatedDate = DateTime.UtcNow,
            ModifiedDate = DateTime.UtcNow
        };
        return await repository.CreateAsync(patient);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePatientRequestDto dto)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null) return false;

        existing.FirstName = dto.FirstName ?? existing.FirstName;
        existing.LastName = dto.LastName ?? existing.LastName;
        existing.DateOfBirth = dto.DateOfBirth ?? existing.DateOfBirth;
        existing.SocialSecurityNumber = dto.SocialSecurityNumber ?? existing.SocialSecurityNumber;
        existing.Address = dto.Address ?? existing.Address;
        existing.PhoneNumber = dto.PhoneNumber ?? existing.PhoneNumber;
        existing.Email = dto.Email ?? existing.Email;
        existing.ModifiedDate = DateTime.UtcNow;

        return await repository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await repository.DeleteAsync(id);
    }

    private PatientResponseDto MapToResponse(Patient patient) => new(
        patient.Id,
        patient.FirstName,
        patient.LastName,
        patient.DateOfBirth.ToString("yyyy-MM-dd"),
        patient.SocialSecurityNumber,
        patient.Address,
        patient.PhoneNumber,
        patient.Email,
        patient.CreatedDate,
        patient.ModifiedDate
    );
}