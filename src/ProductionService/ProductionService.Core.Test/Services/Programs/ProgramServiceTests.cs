using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Programs;
using ProductionService.Shared.Dtos.Programs;
using Xunit;

namespace ProductionService.Core.Test.Services.Programs
{
    public class ProgramServiceTests
    {
        // Service to test.
        private readonly ProgramService _programService;

        // Injected services.
        private readonly Mock<IProgramValidator> _programValidator;
        private readonly Mock<IProgramRepository> _programRepository;

        public ProgramServiceTests()
        {
            _programValidator = new Mock<IProgramValidator>();
            _programRepository = new();
            _programService = new ProgramService(_programRepository.Object, _programValidator.Object);
        }

        #region GetProgramAsync

        [Fact]
        public async Task GetProgramAsync_ReturnsNothing()
        {
            // Arrange
            _programValidator.Setup(r => r.FindByKeyIdValidateAsync(ProgramValidatorTestsData.ProgramKeyId, null, 1)).ReturnsAsync(
                await Task.FromResult<ProgramModel>(null));

            // Act
            ProgramModel programModel = await _programService.GetProgramAsync(ProgramValidatorTestsData.ProgramKeyId);

            // Assert
            Assert.Null(programModel);
        }

        [Fact]
        public async Task GetProgramAsync_ReturnsProgramModel()
        {
            // Arrange
            ProgramModel programModelMock = new()
            {
                KeyId = ProgramValidatorTestsData.ProgramKeyId
            };
            _programValidator.Setup(r => r.FindByKeyIdValidateAsync(ProgramValidatorTestsData.ProgramKeyId, _programRepository.Object.GetProgramAsync, 1))
                .ReturnsAsync(await Task.FromResult(programModelMock));

            // Act
            ProgramModel programModel = await _programService.GetProgramAsync(ProgramValidatorTestsData.ProgramKeyId);

            // Assert
            Assert.NotNull(programModel);
            Assert.Equal(ProgramValidatorTestsData.ProgramKeyId, programModel.KeyId);
        }

        #endregion GetProgramAsync

        #region GetProgramsByMachineAsync

        [Fact]
        public async Task GetProgramsByMachineAsync_ReturnsEmptyListOfProgramBasicDetails()
        {
            // Arrange
            _programValidator.Setup(r => r.ProgramsByMachineValidateAsync(ProgramValidatorTestsData.MachineKeyId)).ReturnsAsync(
                await Task.FromResult<IList<ProgramDetailsBaseDto>>(new List<ProgramDetailsBaseDto>()));

            // Act
            IList<ProgramDetailsBaseDto> programs = await _programService.GetProgramsByMachineAsync(ProgramValidatorTestsData.MachineKeyId);

            // Assert
            Assert.Empty(programs);
        }

        [Fact]
        public async Task GetProgramsByMachineAsync_ReturnsListOfProgramBasicDetails()
        {
            // Arrange
            _programValidator.Setup(r => r.ProgramsByMachineValidateAsync(ProgramValidatorTestsData.MachineKeyId)).ReturnsAsync(
                await Task.FromResult<IList<ProgramDetailsBaseDto>>(new List<ProgramDetailsBaseDto> { new ProgramDetailsBaseDto() }));

            // Act
            IList<ProgramDetailsBaseDto> programs = await _programService.GetProgramsByMachineAsync(ProgramValidatorTestsData.MachineKeyId);

            // Assert
            Assert.NotEmpty(programs);
        }

        #endregion GetProgramsByMachineAsync

        #region GetProgramForMachineAsync

        [Fact]
        public async Task GetProgramForMachineAsync_ReturnsNothing()
        {
            // Arrange
            _programValidator.Setup(r => r.ProgramForMachineValidateAsync(ProgramValidatorTestsData.ProgramKeyId, ProgramValidatorTestsData.MachineKeyId)).ReturnsAsync(
                await Task.FromResult<ProgramDetailsDto>(null));

            // Act
            ProgramDetailsBaseDto programDetails = await _programService.GetProgramForMachineAsync(ProgramValidatorTestsData.ProgramKeyId, ProgramValidatorTestsData.MachineKeyId);

            // Assert
            Assert.Null(programDetails);
        }

        [Fact]
        public async Task GetProgramForMachineAsync_ReturnsProgramInfo()
        {
            // Arrange
            ProgramDetailsBaseDto programDetailsMock = new()
            {
                KeyId = ProgramValidatorTestsData.ProgramKeyId,
                Name = ProgramValidatorTestsData.ProgramName,
                Text = ProgramValidatorTestsData.ProgramText
            };
            _programValidator.Setup(r => r.ProgramForMachineValidateAsync(ProgramValidatorTestsData.ProgramKeyId, ProgramValidatorTestsData.MachineKeyId)).ReturnsAsync(
                await Task.FromResult(programDetailsMock));

            // Act
            ProgramDetailsBaseDto programDetails = await _programService.GetProgramForMachineAsync(ProgramValidatorTestsData.ProgramKeyId, ProgramValidatorTestsData.MachineKeyId);

            // Assert
            Assert.True(
                programDetails != null
                && programDetails.KeyId == ProgramValidatorTestsData.ProgramKeyId
                && programDetails.Name == ProgramValidatorTestsData.ProgramName
                && programDetails.Text == ProgramValidatorTestsData.ProgramText);
        }

        #endregion GetProgramForMachineAsync
    }
}