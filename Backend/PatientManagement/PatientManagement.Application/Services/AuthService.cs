using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PatientManagement.Application.DTOs.User;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PatientManagement.Application.Services;

public class AuthService(
    IConfiguration config,
    IUserRepository userRepository) : IAuthService
{
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await userRepository.GetByEmailAsync(request.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(2),
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await userRepository.UpdateAsync(user);

        return new LoginResponseDto(
            Token: jwtToken,
            Expiration: token.ValidTo,
            Email: user.Email,
            RefreshToken: refreshToken,
            RefreshTokenExpiration: user.RefreshTokenExpiryTime.Value
        );
    }

    public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        var user = await userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user is null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(2),
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        var newRefreshToken = GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await userRepository.UpdateAsync(user);

        return new LoginResponseDto(
            Token: jwtToken,
            Expiration: token.ValidTo,
            Email: user.Email,
            RefreshToken: newRefreshToken,
            RefreshTokenExpiration: user.RefreshTokenExpiryTime.Value
        );
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task<bool> RegisterAsync(RegisterRequestDto request)
    {
        var exists = await userRepository.EmailExistsAsync(request.Email);
        if (exists) return false;

        var user = new User
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role
        };

        await userRepository.CreateAsync(user);
        return true;
    }
}
