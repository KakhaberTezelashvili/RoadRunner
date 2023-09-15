using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Programs;
using ProductionService.Shared.Dtos.Machines;
using ProductionService.Shared.Dtos.Programs;
using Xunit;

namespace ProductionService.Core.Test.Services.Programs
{
    public class ProgramValidatorTests
    {
        // Service to test.
        private readonly ProgramValidator _programValidator;

        // Injected services.
        private readonly Mock<IProgramRepository> _programRepository;

        public ProgramValidatorTests()
        {
            _programRepository = new Mock<IProgramRepository>();
            _programValidator = new ProgramValidator(_programRepository.Object);
        }

        #region ProgramForMachineValidateAsync

        [Fact]
        [Trait("Category", "ProgramValidator.ProgramForMachineValidateAsync")]
        public async Task ProgramForMachineValidateAsync_ArgsKeyIdsAreZero_ThrowsException()
        {
            // Arrange
            int machineKeyId = 0;
            int programKeyId = 0;

            // Act
            var exception = await Record.ExceptionAsync(() => _programValidator.ProgramForMachineValidateAsync(machineKeyId, programKeyId)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProgramValidator.ProgramForMachineValidateAsync")]
        public async Task ProgramForMachineValidateAsync_ProgramNotExist_ThrowsException()
        {
            // Arrange
            int machineKeyId = 1;
            int programKeyId = 1;
            ProgramDetailsDto mockProgram = default;

            _programRepository.Setup(r => r.GetProgramDetailsAsync(programKeyId, true))
                .ReturnsAsync(await Task.FromResult(mockProgram));

            // Act
            var exception = await Record.ExceptionAsync(() => _programValidator.ProgramForMachineValidateAsync(programKeyId, machineKeyId)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProgramValidator.ProgramForMachineValidateAsync")]
        public async Task ProgramForMachineValidateAsync_ProgramNotValid_ThrowsException()
        {
            // Arrange
            int machineKeyId = 1;
            int programKeyId = 0;
            ProgramDetailsDto mockProgram = new();

            _programRepository.Setup(r => r.GetProgramDetailsAsync(programKeyId, true))
                .ReturnsAsync(await Task.FromResult(mockProgram));

            // Act
            var exception = await Record.ExceptionAsync(() => _programValidator.ProgramForMachineValidateAsync(programKeyId, machineKeyId)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProgramValidator.ProgramForMachineValidateAsync")]
        public async Task ProgramForMachineValidateAsync_ProgramIsValid_NotThrowsException()
        {
            // Arrange
            int machineKeyId = 1;
            int programKeyId = 1;
            ProgramDetailsDto mockProgram = new()
            {
                Machines = new List<MachineDetailsBaseDto>()
                    {
                        new MachineDetailsBaseDto() { KeyId = machineKeyId}
                    }
            };

            _programRepository.Setup(r => r.FindByKeyIdAsync(programKeyId))
                .ReturnsAsync(await Task.FromResult(new ProgramModel()));
            _programRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<MachineModel>(machineKeyId))
                .ReturnsAsync(await Task.FromResult(new MachineModel()));
            _programRepository.Setup(r => r.GetProgramDetailsAsync(programKeyId, true))
                .ReturnsAsync(await Task.FromResult(mockProgram));

            // Act
            Exception exception = await Record.ExceptionAsync(() => _programValidator.ProgramForMachineValidateAsync(machineKeyId, programKeyId));

            // Assert
            Assert.Null(exception);
        }

        #endregion ProgramForMachineValidateAsync

        #region ProgramsByMachineValidateAsync

        [Fact]
        [Trait("Category", "ProgramValidator.ProgramsByMachineValidateAsync")]
        public async Task ProgramsByMachineValidateAsync_ArgsKeyIdsAreZero_ThrowsException()
        {
            // Arrange
            int machineKeyId = 0;

            // Act
            var exception = await Record.ExceptionAsync(() => _programValidator.ProgramsByMachineValidateAsync(machineKeyId)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProgramValidator.ProgramsByMachineValidateAsync")]
        public async Task ProgramsByMachineValidateAsync_ProgramArgumentsIsValid_NotThrowsException()
        {
            // Arrange
            int machineKeyId = 1;
            _programRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<MachineModel>(machineKeyId))
                .ReturnsAsync(await Task.FromResult(new MachineModel()));

            // Act
            Exception exception = await Record.ExceptionAsync(() => _programValidator.ProgramsByMachineValidateAsync(machineKeyId));

            // Assert
            Assert.Null(exception);
        }

        #endregion ProgramsByMachineValidateAsync
    }
}