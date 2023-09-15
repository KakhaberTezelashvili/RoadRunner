using ProductionService.Infrastructure.Repositories;
using ProductionService.Shared.Dtos.Patients;

namespace ProductionService.Infrastructure.Test.Repositories;

public class PatientRepositoryTests : RepositoryBaseTests
{
    #region GetBasicInfoByKeyIdAsync

    [Theory]
    [InlineData(1000)]
    public async Task GetBasicInfoByKeyIdAsync_ReturnsNothing(int patientKeyId)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var patientRepository = new PatientRepository(context);

        // Act
        PatientDetailsBaseDto patient = await patientRepository.GetBasicInfoByKeyIdAsync(patientKeyId);

        // Assert
        Assert.Null(patient);
    }

    [Theory]
    [InlineData(1002, "110166-xxxx")]
    [InlineData(1001, "PAT-00001")]
    public async Task GetBasicInfoByKeyIdAsync_ReturnsPatientBasicInfo(int patientKeyId, string id)
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var patientModel = new PatientModel
        {
            KeyId = patientKeyId,
            Id = id
        };

        await context.AddAsync(patientModel);
        await context.SaveChangesAsync();
        var patientRepository = new PatientRepository(context);

        // Act
        PatientDetailsBaseDto patient = await patientRepository.GetBasicInfoByKeyIdAsync(patientKeyId);
        context.Remove(patientModel);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(patient);
        Assert.Equal(patientKeyId, patient.KeyId);
        Assert.Equal(id, patient.Id);
    }

    #endregion GetBasicInfoByKeyIdAsync

    #region GetAllBasicInfosAsync

    [Fact]
    public async Task GetAllBasicInfosAsync_ReturnsPatientsBasicInfo()
    {
        // Arrange
        await using TDocEFDbContext context = ConfigureContext();
        var patientModels = new List<PatientModel>
        {
            new()
            {
                KeyId = 1001,
                Id = "PAT-00001"
            },
            new()
            {
                KeyId = 1002,
                Id = "110166-xxxx"
            },
        };
        patientModels = patientModels.OrderBy(p => p.Id).ToList();
        await context.Patients.AddRangeAsync(patientModels);
        await context.SaveChangesAsync();
        var patientRepository = new PatientRepository(context);

        // Act
        IList<PatientDetailsBaseDto> patients = await patientRepository.GetAllBasicInfosAsync();
        context.RemoveRange(patientModels);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotNull(patients);
        Assert.Equal(patientModels.Count, patients.Count);
        Assert.Equal(patientModels[0].KeyId, patients[0].KeyId);
        Assert.Equal(patientModels[1].KeyId, patients[1].KeyId);
        Assert.Equal(patientModels[0].Id, patients[0].Id);
        Assert.Equal(patientModels[1].Id, patients[1].Id);
    }

    #endregion GetAllBasicInfosAsync
}