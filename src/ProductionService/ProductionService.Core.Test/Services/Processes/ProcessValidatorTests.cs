using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.PositionLocations;
using ProductionService.Core.Services.Processes;
using ProductionService.Core.Services.Texts;
using ProductionService.Shared.Dtos.Processes;
using Xunit;

namespace ProductionService.Core.Test.Services.Processes
{
    public class ProcessValidatorTests
    {
        private const int _batchKeyId = 1;
        private const int _error = 1;
        private const int _locationKeyId = 1;
        private const int _positionLocationKeyId = 1;
        private const int _userKeyId = 1;
        private readonly BatchApproveArgs _batchApproveArgs = new(_locationKeyId, _positionLocationKeyId);

        private readonly MachineType _machineType = MachineType.Washer;

        // Service to test.
        private readonly IProcessValidator _processValidator;

        // Injected services.
        private readonly Mock<ITextValidator> _textValidator;
        private readonly Mock<IPositionLocationValidator> _positionLocationValidator;
        private readonly Mock<IProcessRepository> _processRepository;

        // Constructor
        public ProcessValidatorTests()
        {
            _textValidator = new();
            _positionLocationValidator = new();
            _processRepository = new();
            _processValidator = new ProcessValidator(_processRepository.Object, _textValidator.Object);
        }

        #region CreateBatchValidateAsync

        [Fact]
        [Trait("Category", "ProcessValidator.CreateBatchValidateAsync")]
        public async Task CreateBatchValidateAsync_BatchCreateArgsIsNull_ThrowsException()
        {
            // Arrange

            // Act
            var exception = await Record.ExceptionAsync(async () => await _processValidator.CreateBatchValidateAsync(null)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNull, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.CreateBatchValidateAsync")]
        public async Task CreateBatchValidateAsync_BatchCreateArgsIsInvalid_ThrowsException()
        {
            // Arrange
            BatchCreateArgs mockArgs = new()
            {
                MachineKeyId = 0,
                ProgramKeyId = 0,
                LocationKeyId = 0,
                PositionLocationKeyId = 0
            };

            // Act
            var exception = await Record.ExceptionAsync(async () => await _processValidator.CreateBatchValidateAsync(mockArgs)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.CreateBatchValidateAsync")]
        public async Task CreateBatchValidateAsync_MachineNotSelected_ThrowsException()
        {
            // Arrange
            BatchCreateArgs mockArgs = new()
            {
                MachineKeyId = 0,
                ProgramKeyId = 1,
                LocationKeyId = 1,
                PositionLocationKeyId = 1
            };

            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<ProgramModel>(mockArgs.ProgramKeyId))
                .ReturnsAsync(new ProgramModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(mockArgs.LocationKeyId))
                .ReturnsAsync(new LocationModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PosLocationModel>(mockArgs.PositionLocationKeyId))
                .ReturnsAsync(new PosLocationModel());

            // Act
            var exception = await Record.ExceptionAsync(async () => await _processValidator.CreateBatchValidateAsync(mockArgs)) as DomainException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(DomainBatchErrorCodes.MachineNotSelected, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.CreateBatchValidateAsync")]
        public async Task CreateBatchValidateAsync_BatchCreateArgsIsValid_NotThrowsException()
        {
            // Arrange
            BatchCreateArgs mockArgs = new()
            {
                MachineKeyId = 1,
                ProgramKeyId = 1,
                LocationKeyId = 1,
                PositionLocationKeyId = 1
            };

            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<ProgramModel>(mockArgs.ProgramKeyId))
                .ReturnsAsync(new ProgramModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(mockArgs.LocationKeyId))
                .ReturnsAsync(new LocationModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PosLocationModel>(mockArgs.PositionLocationKeyId))
                .ReturnsAsync(new PosLocationModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<MachineModel>(mockArgs.MachineKeyId))
                .ReturnsAsync(new MachineModel());

            // Act
            Exception exception = await Record.ExceptionAsync(async () => await _processValidator.CreateBatchValidateAsync(mockArgs));

            // Assert
            Assert.Null(exception);
        }

        #endregion CreateBatchValidateAsync

        #region ApproveBatchValidateAsync

        [Fact]
        [Trait("Category", "ProcessValidator.ApproveBatchValidateAsync")]
        public async Task ApproveBatchValidateAsync_BatchApproveArgsIsNull_ThrowsException()
        {
            // Arrange

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.ApproveBatchValidateAsync(-_batchKeyId, _userKeyId, null)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNull, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.ApproveBatchValidateAsync")]
        public async Task ApproveBatchValidateAsync_BatchKeyIdIsLessThanZero_ThrowsException()
        {
            // Arrange

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.ApproveBatchValidateAsync(-1, _userKeyId, _batchApproveArgs)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.ApproveBatchValidateAsync")]
        public async Task ApproveBatchValidateAsync_UserKeyIdIsLessThanZero_ThrowsException()
        {
            // Arrange
            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.ApproveBatchValidateAsync(_batchKeyId, -1, _batchApproveArgs)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.ApproveBatchValidateAsync")]
        public async Task ApproveBatchValidateAsync_LocationKeyIdIsLessThanZero_ThrowsException()
        {
            // Arrange
            var args = new BatchApproveArgs(-1, _positionLocationKeyId);
            ProcessModel process = new() { Type = (int)_machineType };
            LocationModel location = new() { Process = ProcessType.WashPostBatchWF };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PosLocationModel>(_positionLocationKeyId))
                .ReturnsAsync(new PosLocationModel());

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.ApproveBatchValidateAsync(_batchKeyId, _userKeyId, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.ApproveBatchValidateAsync")]
        public async Task ApproveBatchValidateAsync_PositionLocationKeyIdIsLessThanZero_ThrowsException()
        {
            // Arrange
            var args = new BatchApproveArgs(_locationKeyId, -1);
            ProcessModel process = new() { Type = (int)_machineType };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.ApproveBatchValidateAsync(_batchKeyId, _userKeyId, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.ApproveBatchValidateAsync")]
        public async Task ApproveBatchValidateAsync_ProcessNotFound_ThrowsException()
        {
            // Arrange
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult((ProcessModel)null));

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.ApproveBatchValidateAsync(_batchKeyId, _userKeyId, _batchApproveArgs)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
        }

        [Theory]
        [InlineData(MachineType.EndoDryer)]
        [InlineData(MachineType.EndoWasher)]
        [InlineData(MachineType.Incubator)]
        [InlineData(MachineType.PreDisinfector)]
        [Trait("Category", "ProcessValidator.ApproveBatchValidateAsync")]
        public async Task ApproveBatchValidateAsync_MachineTypeIsNotSterilizerOrWasher_ThrowsException(MachineType machineType)
        {
            // Arrange
            ProcessModel process = new() { Type = (int)machineType };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PosLocationModel>(_positionLocationKeyId))
                .ReturnsAsync(new PosLocationModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(_locationKeyId))
                .ReturnsAsync(new LocationModel());

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.ApproveBatchValidateAsync(_batchKeyId, _userKeyId, _batchApproveArgs)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.ApproveBatchValidateAsync")]
        public async Task ApproveBatchValidateAsync_ProcessTypeIsNull_ThrowsException()
        {
            // Arrange
            ProcessModel process = new() {};

            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.ApproveBatchValidateAsync(_batchKeyId, _userKeyId, _batchApproveArgs)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.ApproveBatchValidateAsync")]
        public async Task ApproveBatchValidateAsync_BatchAlreadyApproved_ThrowsException()
        {
            // Arrange
            ProcessModel process = new()
            {
                Type = (int)_machineType,
                ApproveTime = DateTime.Now
            };
            LocationModel location = new() { Process = ProcessType.WashPostBatchWF };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PosLocationModel>(_positionLocationKeyId))
                .ReturnsAsync(new PosLocationModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(_locationKeyId))
                .ReturnsAsync(await Task.FromResult(location));

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.ApproveBatchValidateAsync(_batchKeyId, _userKeyId, _batchApproveArgs)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.ApproveBatchValidateAsync")]
        public async Task ApproveBatchValidateAsync_ProcessIsValid_NotThrowsException()
        {
            // Arrange
            ProcessModel process = new() { Type = (int)_machineType };
            LocationModel location = new() { Process = ProcessType.WashPostBatchWF };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PosLocationModel>(_positionLocationKeyId))
                .ReturnsAsync(new PosLocationModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(_locationKeyId))
                .ReturnsAsync(await Task.FromResult(location));

            // Act
            Exception exception = await Record.ExceptionAsync(() => _processValidator.ApproveBatchValidateAsync(_batchKeyId, _userKeyId, _batchApproveArgs));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.ApproveBatchValidateAsync")]
        public async Task ApproveBatchValidateAsync_LocationAndProcessTypeDoesNotMatch_ThrowsException()
        {
            // Arrange
            ProcessModel process = new() { Type = (int)MachineType.Sterilizer };
            LocationModel location = new() { Process = ProcessType.WashPostBatchWF };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PosLocationModel>(_positionLocationKeyId))
                .ReturnsAsync(new PosLocationModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(_locationKeyId))
                .ReturnsAsync(await Task.FromResult(location));

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.ApproveBatchValidateAsync(_batchKeyId, _userKeyId, _batchApproveArgs)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.ApproveBatchValidateAsync")]
        public async Task ApproveBatchValidateAsync_LocationAndProcessTypeMatches_NotThrowsException()
        {
            // Arrange
            ProcessModel process = new() { Type = (int)MachineType.Washer };
            LocationModel location = new() { Process = ProcessType.WashPostBatchWF };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PosLocationModel>(_positionLocationKeyId))
                .ReturnsAsync(new PosLocationModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(_locationKeyId))
                .ReturnsAsync(await Task.FromResult(location));

            // Act
            Exception exception = await Record.ExceptionAsync(() => _processValidator.ApproveBatchValidateAsync(_batchKeyId, _userKeyId, _batchApproveArgs));

            // Assert
            Assert.Null(exception);
        }

        #endregion ApproveBatchValidateAsync

        #region DisapproveBatchValidateAsync

        [Fact]
        [Trait("Category", "ProcessValidator.DisapproveBatchValidateAsync")]
        public async Task DisapproveBatchValidateAsync_BatchNoIsLessThanZero_ThrowsException()
        {
            // Arrange
            BatchDisapproveArgs args = new(_locationKeyId, _positionLocationKeyId, _error);

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.DisapproveBatchValidateAsync(-1, _userKeyId, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.DisapproveBatchValidateAsync")]
        public async Task DisapproveBatchValidateAsync_UserKeyIdIsLessThanZero_ThrowsException()
        {
            // Arrange
            BatchDisapproveArgs args = new(_locationKeyId, _positionLocationKeyId, _error);
            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.DisapproveBatchValidateAsync(_batchKeyId, -1, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.DisapproveBatchValidateAsync")]
        public async Task DisapproveBatchValidateAsync_ErrorIsZero_ThrowsException()
        {
            // Arrange
            BatchDisapproveArgs args = new(_locationKeyId, _positionLocationKeyId, 0);
            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _textValidator.Setup(v => v.GetErrorValidateAsync(args.Error, 1))
                .ThrowsAsync(new InputArgumentException(GenericErrorCodes.ArgumentsNotValid));

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.DisapproveBatchValidateAsync(_batchKeyId, _userKeyId, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.DisapproveBatchValidateAsync")]
        public async Task DisapproveBatchValidateAsync_LocationKeyIdIsLessThanZero_ThrowsException()
        {
            // Arrange
            BatchDisapproveArgs args = new(-1, _positionLocationKeyId, _error);
            ProcessModel process = new() { Type = (int)_machineType };
            LocationModel location = new() { Process = ProcessType.WashPostBatchWF };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.DisapproveBatchValidateAsync(_batchKeyId, _userKeyId, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.DisapproveBatchValidateAsync")]
        public async Task DisapproveBatchValidateAsync_PositionLocationKeyIdIsLessThanZero_ThrowsException()
        {
            // Arrange
            BatchDisapproveArgs args = new(_locationKeyId, -1, _error);
            ProcessModel process = new() { Type = (int)_machineType };
            LocationModel location = new() { Process = ProcessType.WashPostBatchWF };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(args.LocationKeyId))
                .ReturnsAsync(await Task.FromResult(location));
            
            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.DisapproveBatchValidateAsync(_batchKeyId, _userKeyId, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.DisapproveBatchValidateAsync")]
        public async Task DisapproveBatchValidateAsync_ProcessTypeIsNull_ThrowsException()
        {
            // Arrange
            BatchDisapproveArgs args = new(_locationKeyId, _positionLocationKeyId, _error);
            ProcessModel process = new();

            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.DisapproveBatchValidateAsync(_batchKeyId, _userKeyId, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
        }

        [Theory]
        [InlineData(MachineType.EndoDryer)]
        [InlineData(MachineType.EndoWasher)]
        [InlineData(MachineType.Incubator)]
        [InlineData(MachineType.PreDisinfector)]
        [Trait("Category", "ProcessValidator.DisapproveBatchValidateAsync")]
        public async Task DisapproveBatchValidateAsync_MachineTypeIsNotSterilizerOrWasher_ThrowsException(MachineType machineType)
        {
            // Arrange
            BatchDisapproveArgs args = new(_locationKeyId, _positionLocationKeyId, _error);
            LocationModel location = new() { Process = ProcessType.WashPostBatchWF };
            ProcessModel process = new() { Type = (int)machineType };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(args.LocationKeyId))
                .ReturnsAsync(await Task.FromResult(location));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PosLocationModel>(args.PositionLocationKeyId))
                .ReturnsAsync(new PosLocationModel());

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.DisapproveBatchValidateAsync(_batchKeyId, _userKeyId, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.DisapproveBatchValidateAsync")]
        public async Task DisapproveBatchValidateAsync_ProcessNotFound_ThrowsException()
        {
            // Arrange
            BatchDisapproveArgs args = new(_locationKeyId, _positionLocationKeyId, _error);

            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult((ProcessModel)null));

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.DisapproveBatchValidateAsync(_batchKeyId, _userKeyId, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.DisapproveBatchValidateAsync")]
        public async Task DisapproveBatchValidateAsync_BatchAlreadyApprovedDisapproved_ThrowsException()
        {
            // Arrange
            BatchDisapproveArgs args = new(_locationKeyId, _positionLocationKeyId, _error);
            LocationModel location = new() { Process = ProcessType.WashPostBatchWF };
            ProcessModel process = new()
            {
                ApproveTime = DateTime.Now,
                Type = (int)_machineType
            };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(args.LocationKeyId))
                .ReturnsAsync(await Task.FromResult(location));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PosLocationModel>(args.PositionLocationKeyId))
                .ReturnsAsync(new PosLocationModel());

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.DisapproveBatchValidateAsync(_batchKeyId, _userKeyId, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.DisapproveBatchValidateAsync")]
        public async Task DisapproveBatchValidateAsync_ProcessIsValid_NotThrowsException()
        {
            // Arrange
            BatchDisapproveArgs args = new(_locationKeyId, _positionLocationKeyId, _error);
            ProcessModel process = new() { Type = (int)_machineType };
            LocationModel location = new() { Process = ProcessType.WashPostBatchWF };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(args.LocationKeyId))
                .ReturnsAsync(await Task.FromResult(location));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PosLocationModel>(args.PositionLocationKeyId))
                .ReturnsAsync(new PosLocationModel());

            // Act
            Exception exception = await Record.ExceptionAsync(() => _processValidator.DisapproveBatchValidateAsync(_batchKeyId, _userKeyId, args));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.DisapproveBatchValidateAsync")]
        public async Task DisapproveBatchValidateAsync_LocationAndProcessTypeDoesNotMatch_ThrowsException()
        {
            // Arrange
            BatchDisapproveArgs args = new(_locationKeyId, _positionLocationKeyId, _error);
            ProcessModel process = new() { Type = (int)MachineType.Sterilizer };
            LocationModel location = new() { Process = ProcessType.WashPostBatchWF };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(args.LocationKeyId))
                .ReturnsAsync(await Task.FromResult(location));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PosLocationModel>(args.PositionLocationKeyId))
                .ReturnsAsync(new PosLocationModel());

            // Act
            var exception = await Record.ExceptionAsync(() => _processValidator.DisapproveBatchValidateAsync(_batchKeyId, _userKeyId, args)) as InputArgumentException;

            // Assert
            Assert.NotNull(exception);
            Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
        }

        [Fact]
        [Trait("Category", "ProcessValidator.DisapproveBatchValidateAsync")]
        public async Task DisapproveBatchValidateAsync_LocationAndProcessTypeMatches_NotThrowsException()
        {
            // Arrange
            BatchDisapproveArgs args = new(_locationKeyId, _positionLocationKeyId, _error);
            ProcessModel process = new() { Type = (int)MachineType.Washer };
            LocationModel location = new() { Process = ProcessType.WashPostBatchWF };

            _processRepository.Setup(r => r.FindByKeyIdAsync(_batchKeyId))
                .ReturnsAsync(new ProcessModel());
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(_userKeyId))
                .ReturnsAsync(new UserModel());
            _processRepository.Setup(r => r.GetProcessAsync(_batchKeyId))
                .ReturnsAsync(await Task.FromResult(process));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(args.LocationKeyId))
                .ReturnsAsync(await Task.FromResult(location));
            _processRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PosLocationModel>(args.PositionLocationKeyId))
                .ReturnsAsync(new PosLocationModel());

            // Act
            Exception exception = await Record.ExceptionAsync(() => _processValidator.DisapproveBatchValidateAsync(_batchKeyId, _userKeyId, args));

            // Assert
            Assert.Null(exception);
        }

        #endregion DisapproveBatchValidateAsync
    }
}