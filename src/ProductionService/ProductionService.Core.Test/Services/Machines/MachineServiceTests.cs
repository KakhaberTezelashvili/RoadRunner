using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Machines;
using ProductionService.Shared.Dtos.Machines;
using Xunit;

namespace ProductionService.Core.Test.Services.Machines
{
    public class MachineServiceTests
    {
        private const int _machineKeyId = 1005;
        private const int _locationKeyId = 1001;
        private const string _machineName = "M1";
        private const string _machineText = "Machine 1";
        private const MachineType _machineType = MachineType.Sterilizer;

        private readonly IMachineValidator _machineValidator;

        // Service to test.
        private readonly MachineService _machineService;

        // Injected services.
        private readonly Mock<IMachineRepository> _machineRepository;

        public MachineServiceTests()
        {
            _machineRepository = new Mock<IMachineRepository>();
            _machineValidator = new MachineValidator(_machineRepository.Object);
            _machineService = new MachineService(_machineRepository.Object, _machineValidator);
        }

        #region GetMachineAsync

        [Fact]
        public async Task GetMachineAsync_ReturnsFailedValidateBeforeMachineData()
        {
            // Arrange

            // Act
            Exception exception = await Record.ExceptionAsync(() => _machineService.GetByKeyIdAsync(0));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetMachineAsync_ThrowsNotFoundException()
        {
            // Arrange

            // Act
            var exception = await Record.ExceptionAsync(() => _machineService.GetByKeyIdAsync(_machineKeyId)) as InputArgumentException;
            
            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
        }

        [Fact]
        public async Task GetMachineAsync_ReturnsMachineData()
        {
            // Arrange
            _machineRepository.Setup(r => r.FindByKeyIdAsync(_machineKeyId)).ReturnsAsync(new MachineModel());
            _machineRepository.Setup(r => r.GetWithMachineTypeByIdAsync(_machineKeyId)).ReturnsAsync(
                await Task.FromResult(new MachineModel
                {
                    KeyId = _machineKeyId,
                    Name = _machineName,
                    Text = _machineText
                }));

            // Act
            MachineModel machine = await _machineService.GetByKeyIdAsync(_machineKeyId);

            // Assert
            Assert.NotNull(machine);
            Assert.Equal(_machineKeyId, machine.KeyId);
            Assert.Equal(_machineName, machine.Name);
            Assert.Equal(_machineText, machine.Text);
        }

        #endregion GetMachineAsync

        #region GetMachinesByLocationAsync

        [Fact]
        public async Task GetMachinesByLocationAsync_ReturnsFailedValidateBeforeListOfMachineBasicDetails()
        {
            // Arrange

            // Act
            Exception exception = await Record.ExceptionAsync(() => _machineService.GetMachinesByLocationAsync(0, _machineType));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetMachinesByLocationAsync_ReturnsEmptyListOfMachineBasicDetails()
        {
            // Arrange
            _machineRepository.Setup(r => r.GetByLocationAndTypeAsync(_locationKeyId, _machineType)).ReturnsAsync(
                await Task.FromResult<IList<MachineDetailsBaseDto>>(new List<MachineDetailsBaseDto>()));
            _machineRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(_locationKeyId))
                .ReturnsAsync(new LocationModel());

            // Act
            IList<MachineDetailsBaseDto> machines = await _machineService.GetMachinesByLocationAsync(_locationKeyId, _machineType);

            // Assert
            Assert.Empty(machines);
        }

        [Fact]
        public async Task GetMachinesByLocationAsync_ReturnsListOfMachineBasicDetails()
        {
            // Arrange
            _machineRepository.Setup(r => r.GetByLocationAndTypeAsync(_locationKeyId, _machineType)).ReturnsAsync(
                await Task.FromResult<IList<MachineDetailsBaseDto>>(new List<MachineDetailsBaseDto>
                {
                    new MachineDetailsBaseDto()
                    {
                        KeyId = _machineKeyId,
                        Name = _machineName,
                        Text = _machineText
                    }
                }));
            _machineRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(_locationKeyId))
                .ReturnsAsync(new LocationModel());

            // Act
            IList<MachineDetailsBaseDto> machines = await _machineService.GetMachinesByLocationAsync(_locationKeyId, _machineType);

            // Assert
            Assert.NotNull(machines);
            Assert.Equal(1, machines.Count);
            Assert.Equal(_machineKeyId, machines[0].KeyId);
            Assert.Equal(_machineName, machines[0].Name);
            Assert.Equal(_machineText, machines[0].Text);
        }

        #endregion GetMachinesByLocationAsync

        #region GetMachineInfoForBatchCreatingAsync

        [Fact]
        public async Task GetMachineInfoForBatchCreatingAsync_ReturnsFailedValidateBeforeMachineInfo()
        {
            // Arrange

            // Act
            Exception exception = await Record.ExceptionAsync(() => _machineService.GetMachineInfoForBatchCreatingAsync(0, _locationKeyId));

            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetMachineInfoForBatchCreatingAsync_ReturnsMachineInfo()
        {
            // Arrange
            var machineDetails = new MachineDetailsDto()
            {
                KeyId = _machineKeyId,
                LocationKeyId = _locationKeyId,
                Name = _machineName,
                Text = _machineText
            };
            _machineRepository.Setup(r => r.FindByKeyIdAsync(_machineKeyId))
                .ReturnsAsync(new MachineModel());
            _machineRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(_locationKeyId))
                .ReturnsAsync(new LocationModel());
            _machineRepository.Setup(r => r.GetDetailsByKeyIdAsync(_machineKeyId)).ReturnsAsync(
                await Task.FromResult(machineDetails));

            // Act
            MachineDetailsBaseDto machineInfo = await _machineService.GetMachineInfoForBatchCreatingAsync(_machineKeyId, _locationKeyId);

            // Assert
            Assert.NotNull(machineInfo);
            Assert.Equal(machineDetails.KeyId, machineInfo.KeyId);
            Assert.Equal(machineDetails.Name, machineInfo.Name);
            Assert.Equal(machineDetails.Text, machineInfo.Text);
        }

        #endregion GetMachineInfoForBatchCreatingAsync
    }
}