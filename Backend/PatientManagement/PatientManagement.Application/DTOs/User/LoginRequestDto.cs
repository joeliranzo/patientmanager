using System.ComponentModel.DataAnnotations;

namespace PatientManagement.Application.DTOs.User;

public record LoginRequestDto(
    [Required][EmailAddress] string Email,
    [Required] string Password
);
