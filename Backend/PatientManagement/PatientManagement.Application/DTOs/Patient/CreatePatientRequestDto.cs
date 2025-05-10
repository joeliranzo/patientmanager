using System.ComponentModel.DataAnnotations;

namespace PatientManagement.Application.DTOs.Patient;

public record CreatePatientRequestDto(
    [Required][StringLength(100)] string FirstName,
    [Required][StringLength(100)] string LastName,
    [Required] DateTime DateOfBirth,
    [Required][RegularExpression(@"^\d{3}-\d{2}-\d{4}$")] string SocialSecurityNumber,
    [StringLength(255)] string? Address,
    [Phone] string? PhoneNumber,
    [EmailAddress] string? Email
);