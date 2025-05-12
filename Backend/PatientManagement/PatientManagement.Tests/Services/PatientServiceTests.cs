using Moq;
using PatientManagement.Application.DTOs.Patient;
using PatientManagement.Application.Interfaces;
using PatientManagement.Application.Services;
using PatientManagement.Domain.Entities;
using Xunit;

namespace PatientManagement.Tests.Services;

public class PatientServiceTests
{
    [Fact]
    public async Task CreatePatient_ShouldReturnId()
    {
        var repoMock = new Mock<IPatientRepository>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<Patient>())).ReturnsAsync(1);

        var service = new PatientService(repoMock.Object);
        var dto = new CreatePatientRequestDto("Jane", "Smith", DateTime.Parse("1992-05-12"), "987-65-4321", "456 Lane", "9876543210", "jane@example.com");

        var result = await service.CreateAsync(dto);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task GetPatientById_ShouldReturnCorrectPatient()
    {
        var repoMock = new Mock<IPatientRepository>();
        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Patient
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Smith"
        });

        var service = new PatientService(repoMock.Object);
        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Jane", result!.FirstName);
        Assert.Equal("Smith", result.LastName);
    }

    [Fact]
    public async Task DeletePatient_ShouldReturnTrue()
    {
        var repoMock = new Mock<IPatientRepository>();
        repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var service = new PatientService(repoMock.Object);
        var result = await service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdatePatient_ShouldReturnTrue()
    {
        var repoMock = new Mock<IPatientRepository>();
        repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Patient
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Smith",
            DateOfBirth = new DateTime(1990, 1, 1),
            SocialSecurityNumber = "123-45-6789"
        });

        repoMock.Setup(r => r.UpdateAsync(It.IsAny<Patient>())).ReturnsAsync(true);

        var service = new PatientService(repoMock.Object);
        var updateDto = new UpdatePatientRequestDto("Jane Updated", null, null, null, null, null, null);

        var result = await service.UpdateAsync(1, updateDto);

        Assert.True(result);
    }
}