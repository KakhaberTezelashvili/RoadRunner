using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Batches;
using ProductionService.Core.Services.Machines;
using ProductionService.Core.Services.Processes;
using ProductionService.Core.Services.Units;
using ProductionService.Core.Services.Units.Batches;
using ProductionService.Core.Services.Units.Locations;
using ProductionService.Shared.Dtos.Processes;
using ProductionService.Shared.Dtos.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Processes
{
    public class ProcessServiceTests
    {
        private const int _batchKeyId = 1;
        private const int _invalidBatchKeyId = -1;
        private const int _locationKeyId = 1;
        private const int _machineKeyId = 1;
        private const int _positionLocationKeyId = 1;
        private const int _programKeyId = 1;
        private const int _userKeyId = 1;
        private const int _keyId = 1;

        private readonly IList<int> _unitKeyIds = new List<int>() { 1, 2 };

        // Service to test
        private readonly ProcessService _processService;

        // Injected services
        private readonly Mock<IProcessRepository> _processRepository;
        private readonly Mock<IMachineService> _machineService;
        private readonly Mock<IProcessValidator> _processValidator;
        private readonly Mock<IBatchService> _batchService;
        private readonly Mock<IUnitService> _unitService;
        private readonly Mock<IUnitBatchService> _unitBatchService;
        private readonly Mock<IUnitLocationService> _unitLocationService;

        public ProcessServiceTests()
        {
            _processRepository = new Mock<IProcessRepository>();
            _processValidator = new Mock<IProcessValidator>();
            _machineService = new();
            _batchService = new();
            _unitService = new();
            _unitBatchService = new();
            _unitLocationService = new();
            _processService = new ProcessService(_processRepository.Object, _machineService.Object, _processValidator.Object, 
                _batchService.Object, _unitService.Object, _unitBatchService.Object, _unitLocationService.Object);
        }

        #region CreateBatchAsync

        [Fact]
        [Trait("Category", "ProcessService.CreateBatchAsync")]
        public async Task CreateBatchAsync_BatchCreateArgsNull_ReturnsFailedValidateBeforeBatchNo()
        {
            // Arrange

            // Act
            Exception exception = await Record.ExceptionAsync(() => _processService.CreateBatchAsync(_userKeyId, null));

            // Assert
            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData(MachineType.Washer)]
        [InlineData(MachineType.Sterilizer)]
        [Trait("Category", "ProcessService.CreateBatchAsync")]
        public async Task CreateBatchAsync_BatchCreateArgsIsValid_ReturnsBatchNo(MachineType machineType)
        {
            // Arrange
            MachineTypeModel mockMachineTypeModel = new()
            {
                Type = (int)machineType
            };
            MachineModel mockMachine = new()
            {
                McTyp = mockMachineTypeModel,
                KeyId = _machineKeyId,
                RefNum = 1
            };

            _machineService.Setup(m => m.GetByKeyIdAsync(_machineKeyId))
                .ReturnsAsync(await Task.FromResult(mockMachine));

            BatchCreateArgs mockBatchArgs = new()
            {
                LocationKeyId = _locationKeyId,
                MachineKeyId = _machineKeyId,
                PositionLocationKeyId = _positionLocationKeyId,
                ProgramKeyId = _programKeyId,
                UnitKeyIds = _unitKeyIds
            };

            ProcessModel mockProcessModel = new()
            {
                KeyId = _batchKeyId
            };

            _processRepository.Setup(r => r.CreateProcessAsync(It.IsAny<ProcessModel>()))
                .ReturnsAsync(await Task.FromResult(mockProcessModel.KeyId));

            // Act
            int createdBatchId = await _processService.CreateBatchAsync(_userKeyId, mockBatchArgs);

            // Assert
            Assert.Equal(mockProcessModel.KeyId, createdBatchId);
        }

        #endregion CreateBatchAsync

        #region ApproveBatchAsync

        [Fact]
        [Trait("Category", "ProcessService.ApproveBatchAsync")]
        public async Task ApproveBatchAsync_BatchApproveArgsIsNull_ReturnsFailedValidate()
        {
            // Arrange

            // Act
            Exception exception = await Record.ExceptionAsync(() => _processService.ApproveBatchAsync(_batchKeyId, _userKeyId, null));

            // Assert
            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData(MachineType.Washer)]
        [InlineData(MachineType.Sterilizer)]
        [Trait("Category", "ProcessService.ApproveBatchAsync")]
        public async Task ApproveBatchAsync_ApproveBatchArgsIsValid_ApprovesBatchSuccessfully(MachineType machineType)
        {
            // Arrange
            BatchApproveArgs mockArgs = new(_locationKeyId, _positionLocationKeyId);
            ProcessModel mockProcessModel = new()
            {
                KeyId = _batchKeyId,
                Type = (int)machineType
            };
            var mockUnits = new List<UnitBatchContentsDto>() { new(), new() };
            WhatType mockWhat = (MachineType)mockProcessModel.Type == MachineType.Sterilizer ? WhatType.SteriPreBatch : WhatType.WashPreBatch;

            _processValidator.Setup(r => r.ApproveBatchValidateAsync(_batchKeyId, _userKeyId, mockArgs))
                .ReturnsAsync(await Task.FromResult(mockProcessModel));

            _unitBatchService.Setup(r => r.GetBatchContentsAsync(mockWhat, _batchKeyId, new List<int>(), null))
                .ReturnsAsync(await Task.FromResult(mockUnits));

            // Act
            await _processService.ApproveBatchAsync(_batchKeyId, _userKeyId, mockArgs);

            // Assert
            _processRepository.Verify(r => r.UpdateProcessAsync(It.IsAny<ProcessModel>()), Times.Once);
            _batchService.Verify(r => r.UpdateBatchesStatusToOkAsync(_batchKeyId), Times.Once);
            _unitLocationService.Verify(r => r.AddAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<DateTime>(), It.IsAny<WhatType>(), It.IsAny<int>()), Times.Exactly(mockUnits.Count));
            _unitService.Verify(r => r.UpdateUnitStatusOrErrorAsync(
                It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>()), 
                machineType == MachineType.Sterilizer ? Times.Exactly(mockUnits.Count) : Times.Never());
        }

        #endregion ApproveBatchAsync

        #region DisapproveBatchAsync

        [Fact]
        [Trait("Category", "ProcessService.DisapproveBatchAsync")]
        public async Task DisapproveBatchAsync_BatchDisapproveArgsNull_ReturnsFailedValidateBeforeDisapprovesBatch()
        {
            // Arrange

            // Act
            Exception exception = await Record.ExceptionAsync(() => _processService.DisapproveBatchAsync(_batchKeyId, _userKeyId, null));

            // Assert
            Assert.NotNull(exception);
        }

        [Theory]
        [InlineData(MachineType.Washer)]
        [InlineData(MachineType.Sterilizer)]
        [Trait("Category", "ProcessService.DisapproveBatchAsync")]
        public async Task DisapproveBatchAsync_BatchDisapproveArgsIsValid_DisapprovesBatchSuccessfully(MachineType machineType)
        {
            // Arrange
            BatchDisapproveArgs mockBatchArgs = new(1, _locationKeyId, _positionLocationKeyId);

            ProcessModel mockProcessModel = new()
            {
                KeyId = _batchKeyId,
                Type = (int)machineType
            };
            var mockUnits = new List<UnitBatchContentsDto>() { new(), new() };
            WhatType mockWhat = (MachineType)mockProcessModel.Type == MachineType.Sterilizer ? WhatType.SteriPreBatch : WhatType.WashPreBatch;

            _processValidator.Setup(r => r.DisapproveBatchValidateAsync(_batchKeyId, _userKeyId, mockBatchArgs))
                .ReturnsAsync(await Task.FromResult(mockProcessModel));

            _unitBatchService.Setup(r => r.GetBatchContentsAsync(mockWhat, _batchKeyId, new List<int>(), null))
                .ReturnsAsync(await Task.FromResult(mockUnits));

            // Act
            await _processService.DisapproveBatchAsync(_batchKeyId, _userKeyId, mockBatchArgs);

            // Assert
            _processRepository.Verify(r => r.UpdateProcessAsync(It.IsAny<ProcessModel>()), Times.Once);
            _batchService.Verify(r => r.UpdateBatchesStatusToFailedAsync(_batchKeyId), Times.Once);
            _unitLocationService.Verify(r => r.AddAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), 
                It.IsAny<DateTime>(), It.IsAny<WhatType>(), It.IsAny<int>()), Times.Exactly(mockUnits.Count));
            _unitService.Verify(r => r.UpdateUnitStatusOrErrorAsync(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int?>()), Times.Exactly(mockUnits.Count));
        }

        #endregion DisapproveBatchAsync

        #region GetProcessByBatchKeyIdAsync

        [Fact]
        [Trait("Category", "ProcessService.GetProcessByBatchKeyIdAsync")]
        public async Task GetProcessByBatchKeyIdAsync_ReturnsNothing()
        {
            // Arrange
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId)).ReturnsAsync(
                await Task.FromResult<ProcessModel>(null));

            // Act
            ProcessModel process = await _processService.GetProcessByBatchKeyIdAsync(_batchKeyId);

            // Assert
            Assert.Null(process);
        }

        [Fact]
        [Trait("Category", "ProcessService.GetProcessByBatchKeyIdAsync")]
        public async Task GetProcessByBatchKeyIdAsync_ReturnsProcessModel()
        {
            // Arrange
            ProcessModel mockProcess = new() { KeyId = _keyId };

            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId)).ReturnsAsync(
                await Task.FromResult(mockProcess));

            // Act
            ProcessModel process = await _processService.GetProcessByBatchKeyIdAsync(_batchKeyId);

            // Assert
            Assert.NotNull(process);
            Assert.Equal(process.KeyId, mockProcess.KeyId);
        }

        #endregion GetProcessByBatchKeyIdAsync

        #region GetBatchDetailsAsync

        [Fact]
        [Trait("Category", "ProcessService.GetBatchDetailsAsync")]
        public async Task GetBatchDetailsAsync_ReturnsNothing()
        {
            // Arrange
            _processRepository.Setup(r => r.GetBatchDetailsByBatchKeyIdAsync(_batchKeyId)).ReturnsAsync(
                await Task.FromResult<BatchDetailsDto>(null));

            // Act
            BatchDetailsDto batchDetailsDto = await _processService.GetBatchDetailsAsync(_batchKeyId);

            // Assert
            Assert.Null(batchDetailsDto);
        }

        [Fact]
        [Trait("Category", "ProcessService.GetBatchDetailsAsync")]
        public async Task GetBatchDetailsAsync_ReturnsBatchDetailsDto()
        {
            // Arrange
            BatchDetailsDto mockBatchDetails = new() { Id = _batchKeyId };

            _processRepository.Setup(r => r.GetBatchDetailsByBatchKeyIdAsync(_batchKeyId)).ReturnsAsync(
                await Task.FromResult(mockBatchDetails));

            // Act
            BatchDetailsDto batchDetails = await _processService.GetBatchDetailsAsync(_batchKeyId);

            // Assert
            Assert.NotNull(batchDetails);
            Assert.Equal(batchDetails.Id, mockBatchDetails.Id);
        }

        #endregion GetProcessByBatchKeyIdAsync
    }
}