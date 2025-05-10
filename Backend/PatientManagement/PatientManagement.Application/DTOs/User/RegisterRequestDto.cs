using System.ComponentModel.DataAnnotations;

namespace PatientManagement.Application.DTOs.User;

public record RegisterRequestDto(
    [Required][EmailAddress] string Email,
    [Required][MinLength(6)] string Password,
    string Role = "User"
);
