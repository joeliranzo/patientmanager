namespace PatientManagement.Application.DTOs.Patient;

public record PatientResponseDto(
    int Id,
    string FirstName,
    string LastName,
    string DateOfBirth,
    string SocialSecurityNumber,
    string? Address,
    string? PhoneNumber,
    string? Email,
    DateTime CreatedDate,
    DateTime ModifiedDate
);