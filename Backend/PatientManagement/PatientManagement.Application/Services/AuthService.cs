using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PatientManagement.Application.DTOs.User;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        return new LoginResponseDto(
            Token: tokenHandler.WriteToken(token),
            Expiration: token.ValidTo,
            Email: user.Email
        );
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