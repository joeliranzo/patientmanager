using PatientManagement.Application.DTOs.Patient;
using PatientManagement.Application.DTOs.Shared;

namespace PatientManagement.Application.Interfaces;

public interface IPatientService
{
    Task<IEnumerable<PatientResponseDto>> GetAllAsync();
    Task<PatientResponseDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreatePatientRequestDto dto);
    Task<bool> UpdateAsync(int id, UpdatePatientRequestDto dto);
    Task<bool> DeleteAsync(int id);

    Task<PagedResult<PatientResponseDto>> QueryAsync(PatientQueryParametersDto parameters);
}