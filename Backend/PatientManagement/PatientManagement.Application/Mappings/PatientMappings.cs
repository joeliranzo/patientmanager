namespace PatientManagement.Application.Mappings;

using PatientManagement.Application.DTOs.Patient;
using PatientManagement.Domain.Entities;
using System;

public static class PatientMappings
{
    public static PatientResponseDto ToResponseDto(this Patient patient) => new(
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

    public static Patient ToDomain(this CreatePatientRequestDto dto) => new Patient
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

    public static void ApplyUpdate(this UpdatePatientRequestDto dto, Patient patient)
    {
        if (!string.IsNullOrWhiteSpace(dto.FirstName)) patient.FirstName = dto.FirstName;
        if (!string.IsNullOrWhiteSpace(dto.LastName)) patient.LastName = dto.LastName;
        if (dto.DateOfBirth.HasValue) patient.DateOfBirth = dto.DateOfBirth.Value;
        if (!string.IsNullOrWhiteSpace(dto.SocialSecurityNumber)) patient.SocialSecurityNumber = dto.SocialSecurityNumber;
        if (!string.IsNullOrWhiteSpace(dto.Address)) patient.Address = dto.Address;
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) patient.PhoneNumber = dto.PhoneNumber;
        if (!string.IsNullOrWhiteSpace(dto.Email)) patient.Email = dto.Email;
    }
}
