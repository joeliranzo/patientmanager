using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<bool> EmailExistsAsync(string email);
    Task<int> CreateAsync(User user);
    Task UpdateAsync(User user);
}