using Moq;
using ProductionService.Core.Models.Batches;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Batches;
using ProductionService.Core.Services.Serials;
using ProductionService.Core.Services.Units;
using ProductionService.Shared.Dtos.Processes;
using Xunit;

namespace ProductionService.Core.Test.Services.Batches
{
    public class BatchValidatorTests
    {
        private const int _batchKeyId = 1;
        private const int _unitKeyId = 1;
        private const int _userKeyId = 1;

        private readonly MachineType _machineType = MachineType.Sterilizer;

        // Service to test.
        private readonly IBatchValidator _batchValidator;

        private readonly Mock<IBatchRepository> _batchRepository;
        private readonly Mock<IUnitService> _unitService;
        private readonly Mock<ISerialService> _serialService;

        public BatchValidatorTests()
        {
            _batchRepository = new();
            _unitService = new();
            _serialService = new();
            _batchValidator = new BatchValidator(_batchRepository.Object, _unitService.Object, _serialService.Object);
        }

        #region LinkUnitsToBatchValidateAsync

        [Fact]
        [Trait("Category", "BatchValidator.LinkUnitsToBatchValidateAsync")]
        public async Task LinkUnitsToBatchValidate_BatchCreateArgsIsNull_ThrowsException()
        {
            // Arrange

            // Act
            var exception = await Record.ExceptionAsync(() => _batchValidator.LinkUnitsToBatchValidateAsync(_batchKeyId, _machineType, _userKeyId, null)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNull, exception.Code);
        }

        // TODO: for POC we allowing to create empty batch
        /*
        [Fact]
        [Trait("Category", "BatchValidator.LinkUnitsToBatchValidateAsync")]
        public public void LinkUnitsToBatchValidateAsync_ArgsUnitKeyIdsEmpty_ThrowsException()
        {
            // Arrange
            CreateBatchArgs args = new()
            {
                UnitKeyIds = new List<int>()
            };

            // Act
            var exception = await Record.ExceptionAsync(() => _batchValidator.LinkUnitsToBatchValidateAsync(args, BatchNo, MachineType, UserKeyId)) as DomainException;

            //Assert
            Assert.NotNull(exception);
            Assert.Equal(exception.Code, ErrorCodes.CreateBatchArgumentsNotValid);
        }
        */

        [Fact]
        [Trait("Category", "BatchValidator.LinkUnitsToBatchValidateAsync")]
        public async Task LinkUnitsToBatchValidate_BatchNoIsZero_ThrowsException()
        {
            // Arrange
            BatchCreateArgs args = new()
            {
                UnitKeyIds = new List<int> { 1 }
            };

            // Act
            var exception = await Record.ExceptionAsync(() => _batchValidator.LinkUnitsToBatchValidateAsync(0, _machineType, _userKeyId, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "BatchValidator.LinkUnitsToBatchValidateAsync")]
        public async Task LinkUnitsToBatchValidate_UserKeyIdIsZero_ThrowsException()
        {
            // Arrange
            BatchCreateArgs args = new()
            {
                UnitKeyIds = new List<int> { 1 }
            };

            // Act
            var exception = await Record.ExceptionAsync(() => _batchValidator.LinkUnitsToBatchValidateAsync(_batchKeyId, _machineType, 0, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Theory]
        [InlineData(MachineType.EndoDryer)]
        [InlineData(MachineType.EndoWasher)]
        [InlineData(MachineType.Incubator)]
        [InlineData(MachineType.PreDisinfector)]
        [Trait("Category", "BatchValidator.LinkUnitsToBatchValidateAsync")]
        public async Task LinkUnitsToBatchValidate_MachineTypeIsNotSterilizerOrWasher_ThrowsException(MachineType machineType)
        {
            // Arrange
            BatchCreateArgs args = new()
            {
                UnitKeyIds = new List<int> { 1 }
            };

            // Act
            var exception = await Record.ExceptionAsync(() => _batchValidator.LinkUnitsToBatchValidateAsync(_batchKeyId, machineType, _userKeyId, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "BatchValidator.LinkUnitsToBatchValidateAsync")]
        public async Task LinkUnitsToBatchValidate_ArgsUnitKeyIdsNotEmptyBatchNoIsNotZeroUserKeyIdIsNotZero_NotThrowsException()
        {
            // Arrange
            BatchCreateArgs args = new()
            {
                UnitKeyIds = new List<int> { 1 }
            };
            _batchRepository
                .Setup(br => br.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<BatchType>()))
                .ReturnsAsync(new List<BatchProcessData>());
            _unitService.Setup(us => us.GetByKeyIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<UnitModel> { new() });

            // Act
            Exception exception = await Record.ExceptionAsync(() => _batchValidator.LinkUnitsToBatchValidateAsync(_batchKeyId, _machineType, _userKeyId, args));

            // Assert
            Assert.Null(exception);
        }

        #endregion LinkUnitsToBatchValidateAsync

        #region RemoveBatchesValidate

        [Fact]
        [Trait("Category", "BatchValidator.RemoveBatchesValidate")]
        public void RemoveBatchesValidate_UnitKeyIdIsZero_ThrowsException()
        {
            // Arrange

            // Act
            var exception = Record.Exception(() => _batchValidator.RemoveBatchesValidate(0)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "BatchValidator.RemoveBatchesValidate")]
        public void RemoveBatchesValidate_UnitKeyIdIsNotZero_NotThrowsException()
        {
            // Arrange

            // Act
            Exception exception = Record.Exception(() => _batchValidator.RemoveBatchesValidate(_unitKeyId));

            // Assert
            Assert.Null(exception);
        }

        #endregion RemoveBatchesValidate

        #region AssignUnitsToSterilizeOrWashBatchValidateAsync

        [Fact]
        [Trait("Category", "BatchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync")]
        public async Task AssignUnitsToSterilizeOrWashBatchValidateAsync_MachineTypeIsSterilizerAndUnitsBatchIdAreNull_NotThrowsException()
        {
            // Arrange
            List<int> unitKeyIds = new();
            _unitService.Setup(us => us.GetByKeyIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<UnitModel> { new() { Batch = null }, new() { Batch = null } });

            // Act
            var exception = await Record.ExceptionAsync(() => _batchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync(MachineType.Sterilizer, unitKeyIds)) as InputArgumentException;

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        [Trait("Category", "BatchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync")]
        public async Task AssignUnitsToSterilizeOrWashBatchValidateAsync_MachineTypeIsSterilizerAndUnitsNotAssignedToBatches_NotThrowsException()
        {
            // Arrange
            List<int> unitKeyIds = new();

            _batchRepository
                .Setup(br => br.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<BatchType>()))
                .ReturnsAsync(new List<BatchProcessData>());
            _unitService.Setup(us => us.GetByKeyIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<UnitModel> { new() { Batch = 1 }, new() { Batch = 2 } });

            // Act
            var exception = await Record.ExceptionAsync(() => _batchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync(MachineType.Sterilizer, unitKeyIds)) as InputArgumentException;

            // Assert
            Assert.Null(exception);
            _batchRepository.Verify(b => b.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(It.IsAny<IEnumerable<int>>(), BatchType.PrimarySteri), Times.Once);
        }

        [Fact]
        [Trait("Category", "BatchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync")]
        public async Task AssignUnitsToSterilizeOrWashBatchValidateAsync_UnitAlreadyRegisteredForSterilizeBatch_ThrowsException()
        {
            // Arrange
            List<int> unitKeyIds = new();

            _batchRepository
                .Setup(br => br.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<BatchType>()))
                .ReturnsAsync(new List<BatchProcessData> { new() { BatchKeyId = 1 }, new() { BatchKeyId = 2 } });
            _unitService.Setup(us => us.GetByKeyIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<UnitModel> { new() { Batch = 1 }, new() { Batch = 2 } });

            // Act
            var exception = await Record.ExceptionAsync(() => _batchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync(MachineType.Sterilizer, unitKeyIds)) as DomainException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(DomainBatchErrorCodes.UnitAlreadyRegisteredForSterilizeBatch, exception.Code);
            _batchRepository.Verify(b => b.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(It.IsAny<IEnumerable<int>>(), BatchType.PrimarySteri), Times.Once);
        }

        [Fact]
        [Trait("Category", "BatchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync")]
        public async Task AssignUnitsToSterilizeOrWashBatchValidateAsync_SerialNumberUnitAlreadyRegisteredForSterilizeBatch_ThrowsException()
        {
            // Arrange
            List<int> unitKeyIds = new() { 1, 2 };

            _batchRepository
                .Setup(br => br.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<BatchType>()))
                .ReturnsAsync(new List<BatchProcessData> { new() { BatchKeyId = 1, UnitKeyId = 1 }, new() { BatchKeyId = 2, UnitKeyId = 2 } });
            _unitService.Setup(us => us.GetByKeyIdsAsync(It.IsAny<IEnumerable<int>>()))
                .ReturnsAsync(new List<UnitModel> { new() { KeyId = 1, Batch = 1, SeriKeyId = 1 }, new() { KeyId = 2, Batch = 2, SeriKeyId = 2 } });
            _serialService.Setup(s => s.GetByKeyIdAsync(1))
                .ReturnsAsync(await Task.FromResult(new SerialModel()));

            // Act
            var exception = await Record.ExceptionAsync(() => _batchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync(MachineType.Sterilizer,
                unitKeyIds, true)) as DomainException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(DomainBatchErrorCodes.SerialNumberUnitAlreadyRegisteredForSterilizeBatch, exception.Code);
            _batchRepository.Verify(b => b.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(It.IsAny<IEnumerable<int>>(), BatchType.PrimarySteri), Times.Once);
        }

        [Fact]
        [Trait("Category", "BatchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync")]
        public async Task AssignUnitsToSterilizeOrWashBatchValidateAsync_MachineTypeIsWasherAndUnitsBatchesHaveDoneStatus_NotThrowsException()
        {
            // Arrange
            List<int> unitKeyIds = new() { 1, 2 };

            _batchRepository
                .Setup(br => br.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<BatchType>()))
                .ReturnsAsync(new List<BatchProcessData> { new() { ProcessStatus = ProcessStatus.Done } });

            // Act
            var exception = await Record.ExceptionAsync(() => _batchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync(MachineType.Washer, unitKeyIds)) as DomainException;

            // Assert
            Assert.Null(exception);
            _batchRepository.Verify(b => b.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(It.IsAny<IEnumerable<int>>(), BatchType.PostWash), Times.Once);
        }

        [Fact]
        [Trait("Category", "BatchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync")]
        public async Task AssignUnitsToSterilizeOrWashBatchValidateAsync_BatchUnitAlreadyRegisteredForWashBatch_ThrowsException()
        {
            // Arrange
            List<int> unitKeyIds = new() { 1, 2 };

            _batchRepository
                .Setup(br => br.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<BatchType>()))
                .ReturnsAsync(new List<BatchProcessData>
                {
                    new() { BatchKeyId = 1, UnitKeyId = 1, ProcessStatus = ProcessStatus.Initiated },
                    new() { BatchKeyId = 2, UnitKeyId = 2, ProcessStatus = ProcessStatus.Ending },
                    new() { BatchKeyId = 3, UnitKeyId = 2, ProcessStatus = ProcessStatus.Running }
                });

            // Act
            var exception = await Record.ExceptionAsync(() => _batchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync(MachineType.Washer, unitKeyIds)) as DomainException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(DomainBatchErrorCodes.UnitAlreadyRegisteredForWashBatch, exception.Code);
            _batchRepository.Verify(b => b.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(It.IsAny<IEnumerable<int>>(), BatchType.PostWash), Times.Once);
        }

        [Fact]
        [Trait("Category", "BatchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync")]
        public async Task AssignUnitsToSterilizeOrWashBatchValidateAsync_SerialNumberBatchUnitAlreadyRegisteredForWashBatch_ThrowsException()
        {
            // Arrange
            List<int> unitKeyIds = new() { 1, 2 };

            _batchRepository
                .Setup(br => br.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<BatchType>()))
                .ReturnsAsync(new List<BatchProcessData>
                {
                    new() { BatchKeyId = 1, UnitKeyId = 1, ProcessStatus = ProcessStatus.Initiated },
                    new() { BatchKeyId = 2, UnitKeyId = 2, ProcessStatus = ProcessStatus.Ending },
                    new() { BatchKeyId = 3, UnitKeyId = 2, ProcessStatus = ProcessStatus.Running }
                });
            _unitService.Setup(us => us.GetByKeyIdsAsync(new List<int> { 1 }))
                .ReturnsAsync(new List<UnitModel> { new() { KeyId = 1, Batch = 1, SeriKeyId = 1 } });
            _serialService.Setup(s => s.GetByKeyIdAsync(1))
                .ReturnsAsync(await Task.FromResult(new SerialModel()));

            // Act
            var exception = await Record.ExceptionAsync(() => _batchValidator.AssignUnitsToSterilizeOrWashBatchValidateAsync(MachineType.Washer,
                unitKeyIds, true)) as DomainException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(DomainBatchErrorCodes.SerialNumberUnitAlreadyRegisteredForWashBatch, exception.Code);
            _batchRepository.Verify(b => b.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(It.IsAny<IEnumerable<int>>(), BatchType.PostWash), Times.Once);
        }

        #endregion AssignUnitsToSterilizeOrWashBatchValidateAsync
    }
}