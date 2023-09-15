using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Patients;
using ProductionService.Shared.Dtos.Patients;
using Xunit;

namespace ProductionService.Core.Test.Services.Patients
{
    public class PatientServiceTests
    {
        private const int _patientKeyId = 1;

        private readonly IPatientValidator _patientValidator;

        // Service to test.
        private readonly PatientService _patientService;

        // Injected services.
        private readonly Mock<IPatientRepository> _patientRepository;

        public PatientServiceTests()
        {
            _patientRepository = new Mock<IPatientRepository>();
            _patientValidator = new PatientValidator(_patientRepository.Object);
            _patientService = new PatientService(_patientRepository.Object, _patientValidator);
        }

        #region GetPatientsBasicInfoAsync

        [Fact]
        public async Task GetPatientsBasicInfoAsync_ReturnsEmptyListOfPatientShortDetails()
        {
            // Arrange
            _patientRepository.Setup(r => r.GetAllBasicInfosAsync()).ReturnsAsync(
                await Task.FromResult<IList<PatientDetailsBaseDto>>(new List<PatientDetailsBaseDto>()));
            // Act
            IList<PatientDetailsBaseDto> patients = await _patientService.GetPatientsBasicInfoAsync();
            //Assert
            Assert.Empty(patients);
        }

        [Fact]
        public async Task GetPatientsBasicInfoAsync_ReturnsListOfPatientShortDetails()
        {
            // Arrange
            _patientRepository.Setup(r => r.GetAllBasicInfosAsync()).ReturnsAsync(
                await Task.FromResult<IList<PatientDetailsBaseDto>>(new List<PatientDetailsBaseDto> { new PatientDetailsBaseDto() }));
            // Act
            IList<PatientDetailsBaseDto> patients = await _patientService.GetPatientsBasicInfoAsync();
            //Assert
            Assert.NotEmpty(patients);
        }

        #endregion
    }
}
