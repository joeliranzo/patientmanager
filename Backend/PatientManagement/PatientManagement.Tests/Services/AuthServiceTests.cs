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
        var result = await service.RegisterAsync(new RegisterRequestDto("new@example.com", "pass", "User"));

        Assert.True(result);
    }

    [Fact]
    public async Task LoginUser_ShouldReturnTokens_WhenCredentialsAreValid()
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
        userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:Key"]).Returns("SOME_SECRET_KEY_1234567890123456");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

        var service = new AuthService(configMock.Object, userRepoMock.Object);
        var result = await service.LoginAsync(new LoginRequestDto(user.Email, "password"));

        Assert.NotNull(result);
        Assert.Equal(user.Email, result!.Email);
        Assert.NotNull(result.Token);
        Assert.NotNull(result.RefreshToken);
        Assert.True(result.RefreshTokenExpiration > DateTime.UtcNow);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
    {
        var oldToken = "oldRefresh";
        var user = new User
        {
            Id = 1,
            Email = "refresh@example.com",
            PasswordHash = "hash",
            Role = "User",
            CreatedDate = DateTime.UtcNow,
            RefreshToken = oldToken,
            RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10)
        };

        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.GetByRefreshTokenAsync(oldToken)).ReturnsAsync(user);
        userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:Key"]).Returns("SOME_SECRET_KEY_1234567890123456");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

        var service = new AuthService(configMock.Object, userRepoMock.Object);
        var result = await service.RefreshTokenAsync(oldToken);

        Assert.NotNull(result);
        Assert.NotEqual(oldToken, result!.RefreshToken);
        Assert.True(result.RefreshTokenExpiration > DateTime.UtcNow);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnNull_WhenRefreshTokenIsInvalidOrExpired()
    {
        var token = "invalid";
        var userRepoMock = new Mock<IUserRepository>();
        userRepoMock.Setup(r => r.GetByRefreshTokenAsync(token)).ReturnsAsync((User?)null);

        var configMock = new Mock<IConfiguration>();
        configMock.Setup(c => c["Jwt:Key"]).Returns("SOME_SECRET_KEY_1234567890123456");
        configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
        configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

        var service = new AuthService(configMock.Object, userRepoMock.Object);
        var result = await service.RefreshTokenAsync(token);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginUser_ShouldReturnNull_WhenPasswordIsInvalid()
    {
        var user = new User
        {
            Id = 1,
            Email = "fail@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correct"),
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
        var result = await service.LoginAsync(new LoginRequestDto(user.Email, "wrong"));

        Assert.Null(result);
    }
}
