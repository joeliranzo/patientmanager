using PatientManagement.Application.DTOs.User;

namespace PatientManagement.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<bool> RegisterAsync(RegisterRequestDto request);
    Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
}
