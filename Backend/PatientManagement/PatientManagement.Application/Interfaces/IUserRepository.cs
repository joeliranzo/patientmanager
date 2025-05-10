using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Interfaces;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email);
    Task<int> CreateAsync(User user);
}