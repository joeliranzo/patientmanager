namespace PatientManagement.Application.DTOs.User;

public record LoginResponseDto(
    string Token,
    DateTime Expiration,
    string Email,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);
