using PatientManagement.Application.DTOs.Patient;
using PatientManagement.Application.Interfaces;
using PatientManagement.Application.Mappings;

namespace PatientManagement.Application.Services;

public class PatientService(IPatientRepository repository) : IPatientService
{
    public async Task<IEnumerable<PatientResponseDto>> GetAllAsync() =>
        (await repository.GetAllAsync()).Select(p => p.ToResponseDto());

    public async Task<PatientResponseDto?> GetByIdAsync(int id) =>
        (await repository.GetByIdAsync(id))?.ToResponseDto();

    public async Task<int> CreateAsync(CreatePatientRequestDto dto) =>
        await repository.CreateAsync(dto.ToDomain());

    public async Task<bool> UpdateAsync(int id, UpdatePatientRequestDto dto)
    {
        var existing = await repository.GetByIdAsync(id);
        if (existing is null) return false;
        dto.ApplyUpdate(existing);
        existing.ModifiedDate = DateTime.UtcNow;
        return await repository.UpdateAsync(existing);
    }

    public async Task<bool> DeleteAsync(int id) =>
        await repository.DeleteAsync(id);

    public async Task<IEnumerable<PatientResponseDto>> QueryAsync(PatientQueryParametersDto parameters)
    {
        var patients = await repository.QueryAsync(parameters);
        return patients.Select(p => p.ToResponseDto());
    }

}
