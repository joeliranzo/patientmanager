using Microsoft.Extensions.Configuration;
using Moq;
using PatientManagement.Application.DTOs.User;
using PatientManagement.Application.Interfaces;
using PatientManagement.Application.Services;
using PatientManagement.Domain.Entities;
using Xunit;

namespace PatientManagement.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterUser_ShouldReturnTrue_WhenEmailDoesNotExist()
    {
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        userRepoMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(1);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:Key"]).Returns("SOME_SECRET_KEY_1234567890123456");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

        var service = new AuthService(configMock.Object, userRepoMock.Object);

        var result = await service.RegisterAsync(new RegisterRequestDto("newuser@example.com", "securepassword", "User"));
        Assert.True(result);
    }

    [Fact]
    public async Task LoginUser_ShouldReturnToken_WhenCredentialsAreValid()
    {
        var user = new User
        {
            Id = 1,
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"),
            Role = "Admin",
            CreatedDate = DateTime.UtcNow
        };

        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:Key"]).Returns("SOME_SECRET_KEY_1234567890123456");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

        var service = new AuthService(configMock.Object, userRepoMock.Object);

        var result = await service.LoginAsync(new LoginRequestDto(user.Email, "password"));

        Assert.NotNull(result);
        Assert.Equal(user.Email, result!.Email);
        Assert.NotNull(result.Token);
    }

    [Fact]
    public async Task LoginUser_ShouldReturnNull_WhenInvalidPassword()
    {
        var user = new User
        {
            Id = 1,
            Email = "fail@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct-password"),
            Role = "User",
            CreatedDate = DateTime.UtcNow
        };

        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:Key"]).Returns("SOME_SECRET_KEY_1234567890123456");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

        var service = new AuthService(configMock.Object, userRepoMock.Object);
        var result = await service.LoginAsync(new LoginRequestDto(user.Email, "wrong-password"));

        Assert.Null(result);
    }
}