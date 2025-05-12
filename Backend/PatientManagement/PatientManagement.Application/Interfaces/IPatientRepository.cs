using PatientManagement.Application.DTOs.Patient;
using PatientManagement.Application.DTOs.Shared;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.Interfaces;

public interface IPatientRepository
{
    Task<PagedResult<Patient>> QueryAsync(PatientQueryParametersDto parameters);
    Task<Patient?> GetByIdAsync(int id);
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<int> CreateAsync(Patient patient);
    Task<bool> UpdateAsync(Patient patient);
    Task<bool> DeleteAsync(int id);
}