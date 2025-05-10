using System.ComponentModel.DataAnnotations;

namespace PatientManagement.Application.DTOs.Patient;

public record UpdatePatientRequestDto(
    [StringLength(100)] string? FirstName,
    [StringLength(100)] string? LastName,
    DateTime? DateOfBirth,
    [RegularExpression(@"^\d{3}-\d{2}-\d{4}$")] string? SocialSecurityNumber,
    [StringLength(255)] string? Address,
    [Phone] string? PhoneNumber,
    [EmailAddress] string? Email
);