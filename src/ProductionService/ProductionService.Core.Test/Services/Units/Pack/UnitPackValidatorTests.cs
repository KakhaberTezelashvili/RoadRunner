using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductionService.Core.Models.Batches;
using ProductionService.Core.Models.Units;
using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Core.Services.Batches;
using ProductionService.Core.Services.Serials;
using ProductionService.Core.Services.Units.Pack;
using ProductionService.Shared.Dtos.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Pack;

public class UnitPackValidatorTests
{
    private readonly BatchProcessData _batchProcessData = new()
    {
        ProcessBatch = 1,
        ProcessStatus = ProcessStatus.Done,
        ProcessApprovedUserKeyId = 1
    };
    private readonly List<BatchProcessData> _batchProcesses = new()
    {
        new()
        {
            ProcessBatch = 1,
            ProcessStatus = ProcessStatus.Done,
            ProcessApprovedUserKeyId = 1
        }
    };

    // Service to test.
    private readonly IUnitPackValidator _unitPackValidator;

    // Injected services.
    private readonly Mock<ISerialService> _serialService;
    private readonly Mock<IServiceProvider> _serviceProvider;
    private readonly Mock<IUnitPackRepository> _unitPackRepository;

    public UnitPackValidatorTests()
    {
        _serialService = new();
        _serviceProvider = new();
        _unitPackRepository = new();

        Mock<IServiceScope> serviceScope = new();
        serviceScope.Setup(s => s.ServiceProvider)
            .Returns(_serviceProvider.Object);

        Mock<IServiceScopeFactory> serviceScopeFactory = new();
        serviceScopeFactory.Setup(x => x.CreateScope())
            .Returns(serviceScope.Object);

        _serviceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);

        _unitPackValidator = new UnitPackValidator(_unitPackRepository.Object, _serviceProvider.Object, _serialService.Object);
    }

    #region PackValidateAsync

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_PackUnitArgsIsNull_ThrowsException()
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNull, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_ArgsKeyIdsAreZero_ThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            UnitKeyId = 0,
            ProductKeyId = 0,
            ProductSerialKeyId = 0,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_ArgsLocationFactoryPositionLocationIdsAreZero_ThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            UnitKeyId = 1,
            ProductSerialKeyId = 1,
            ProductKeyId = 1
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_UnitMissingWashBatch_ThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            UnitKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };

        Mock<IBatchService> batchService = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchService)))
            .Returns(batchService.Object);

        _unitPackRepository.Setup(r => r.GetDataToPackAsync(args))
            .ReturnsAsync(await Task.FromResult(new UnitDataToPack { UnitKeyId = 1, UnitStatus = (int)UnitStatus.Returned, WashProgramGroupKeyId = 1 }));
        batchService.Setup(r => r.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(new List<int> { args.UnitKeyId }, BatchType.PostWash))
            .ReturnsAsync(await Task.FromResult((IList<BatchProcessData>)null));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainPackErrorCodes.UnitMissingWashBatch, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_UnitWashBatchNotApproved_ThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            UnitKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };

        List<BatchProcessData> batchProcesses = new()
        {
            new()
            {
                ProcessBatch = 1,
                ProcessStatus = ProcessStatus.Initiated,
                ProcessError = 1
            }
        };

        Mock<IBatchService> batchService = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchService)))
            .Returns(batchService.Object);

        _unitPackRepository.Setup(r => r.GetDataToPackAsync(args))
            .ReturnsAsync(await Task.FromResult(new UnitDataToPack { UnitKeyId = 1, UnitStatus = (int)UnitStatus.Returned, WashProgramGroupKeyId = 1 }));
        batchService.Setup(r => r.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(new List<int> { args.UnitKeyId }, BatchType.PostWash))
            .ReturnsAsync(await Task.FromResult((IList<BatchProcessData>)batchProcesses));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainPackErrorCodes.UnitWashBatchNotApproved, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_ArgsAmountIsZero_ThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            UnitKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            Amount = 0
        };
        List<BatchProcessData> batchProcesses = new()
        {
            _batchProcessData
        };

        Mock<IBatchService> batchService = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchService)))
            .Returns(batchService.Object);

        _unitPackRepository.Setup(r => r.GetDataToPackAsync(args))
            .ReturnsAsync(await Task.FromResult(new UnitDataToPack { UnitKeyId = 1, UnitStatus = (int)UnitStatus.Returned, WashProgramGroupKeyId = 1 }));
        batchService.Setup(r => r.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(new List<int> { args.UnitKeyId }, BatchType.PostWash))
            .ReturnsAsync(await Task.FromResult((IList<BatchProcessData>)batchProcesses));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_ArgsUnitKeyIdsAreNotZero_NotThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            UnitKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };
        List<BatchProcessData> batchProcesses = new()
        {
            _batchProcessData
        };

        Mock<IBatchService> batchService = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchService)))
            .Returns(batchService.Object);

        _unitPackRepository.Setup(r => r.GetDataToPackAsync(args))
            .ReturnsAsync(await Task.FromResult(new UnitDataToPack { UnitKeyId = 1, UnitStatus = (int)UnitStatus.Returned, WashProgramGroupKeyId = 1 }));
        batchService.Setup(r => r.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(new List<int> { args.UnitKeyId }, BatchType.PostWash))
            .ReturnsAsync(await Task.FromResult((IList<BatchProcessData>)batchProcesses));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_ArgsProductSerialKeyIdsAreNotZero_NotThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 1
        };

        Mock<IBatchService> batchService = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchService)))
            .Returns(batchService.Object);

        _unitPackRepository.Setup(r => r.GetDataToPackAsync(args))
            .ReturnsAsync(await Task.FromResult(new UnitDataToPack { UnitKeyId = 1, UnitStatus = (int)UnitStatus.Returned, WashProgramGroupKeyId = 1 }));
        batchService.Setup(r => r.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(new List<int> { 1 }, BatchType.PostWash))
           .ReturnsAsync(await Task.FromResult((IList<BatchProcessData>)_batchProcesses));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_PackDataNotFound_ThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            ProductSerialKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };
        _unitPackRepository.Setup(r => r.GetDataToPackAsync(args))
            .ReturnsAsync(await Task.FromResult((UnitDataToPack)null));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_UnitStatusNotReturned_ThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            UnitKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };

        Mock<IBatchService> batchService = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchService)))
            .Returns(batchService.Object);

        _unitPackRepository.Setup(r => r.GetDataToPackAsync(args))
            .ReturnsAsync(await Task.FromResult(new UnitDataToPack { UnitStatus = (int)It.Is<UnitStatus>(x => x != UnitStatus.Returned), WashProgramGroupKeyId = 1 }));
        batchService.Setup(r => r.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(new List<int> { 0 }, BatchType.PostWash))
           .ReturnsAsync(await Task.FromResult((IList<BatchProcessData>)_batchProcesses));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainPackErrorCodes.UnitStatusNotReturned, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_SerialNumberUnitStatusNotReturned_ThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            ProductSerialKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 1,
        };

        Mock<IBatchService> batchService = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchService)))
            .Returns(batchService.Object);

        _unitPackRepository.Setup(r => r.GetDataToPackAsync(args))
            .ReturnsAsync(await Task.FromResult(new UnitDataToPack { ProductSerialKeyId = 1, UnitStatus = (int)It.Is<UnitStatus>(x => x != UnitStatus.Returned), WashProgramGroupKeyId = 1 }));
        _serialService.Setup(r => r.GetByKeyIdAsync(args.ProductSerialKeyId))
             .ReturnsAsync(await Task.FromResult(new SerialModel() { SerialNo = "1" }));
        batchService.Setup(r => r.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(new List<int> { 1 }, BatchType.PostWash))
           .ReturnsAsync(await Task.FromResult((IList<BatchProcessData>)_batchProcesses));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainPackErrorCodes.SerialNumberUnitStatusNotReturned, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_ArgsAndUnitStatusAreValid_NotThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1,
            UnitKeyId = 1
        };

        Mock<IBatchService> batchService = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchService)))
            .Returns(batchService.Object);

        _unitPackRepository.Setup(r => r.GetDataToPackAsync(args))
            .ReturnsAsync(await Task.FromResult(new UnitDataToPack { UnitKeyId = 1, UnitStatus = (int)UnitStatus.Returned, WashProgramGroupKeyId = 1 }));
        batchService.Setup(r => r.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(new List<int> { 1 }, BatchType.PostWash))
           .ReturnsAsync(await Task.FromResult((IList<BatchProcessData>)_batchProcesses));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_PackBySerialNumber_NotThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            ProductSerialKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };

        Mock<IBatchService> batchService = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchService)))
            .Returns(batchService.Object);

        _unitPackRepository.Setup(r => r.GetDataToPackAsync(args))
            .ReturnsAsync(await Task.FromResult(new UnitDataToPack { UnitStatus = (int)UnitStatus.Returned, WashProgramGroupKeyId = 1, UnitKeyId = 1, ProductSerialKeyId = 1 }));
        batchService.Setup(r => r.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(new List<int> { 1 }, BatchType.PostWash))
           .ReturnsAsync(await Task.FromResult((IList<BatchProcessData>)_batchProcesses));
        _serialService.Setup(r => r.GetByKeyIdAsync(args.ProductSerialKeyId))
            .ReturnsAsync(await Task.FromResult(new SerialModel { Status = (int)TDocConstants.ObjectStatus.Normal }));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_SerialNumberUnitMissingWashBatch_ThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            ProductSerialKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };
        List<BatchProcessData> modifiedBatchesProcessData = new()
        {
            null
        };

        Mock<IBatchService> batchService = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchService)))
            .Returns(batchService.Object);

        _unitPackRepository.Setup(r => r.GetDataToPackAsync(args))
            .ReturnsAsync(await Task.FromResult(new UnitDataToPack { UnitStatus = (int)UnitStatus.Returned, WashProgramGroupKeyId = 1, UnitKeyId = 1, ProductSerialKeyId = 1 }));
        batchService.Setup(r => r.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(new List<int> { 1 }, BatchType.PostWash))
           .ReturnsAsync(await Task.FromResult((IList<BatchProcessData>)modifiedBatchesProcessData));
        _serialService.Setup(r => r.GetByKeyIdAsync(args.ProductSerialKeyId))
            .ReturnsAsync(await Task.FromResult(new SerialModel() { SerialNo = "1" }));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainPackErrorCodes.SerialNumberUnitMissingWashBatch, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_SerialNumberUnitWashBatchNotApproved_ThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            ProductSerialKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };
        List<BatchProcessData> modifiedBatchesProcessData = _batchProcesses;
        modifiedBatchesProcessData.First().ProcessApprovedUserKeyId = null;

        Mock<IBatchService> batchService = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchService)))
            .Returns(batchService.Object);

        _unitPackRepository.Setup(r => r.GetDataToPackAsync(args))
            .ReturnsAsync(await Task.FromResult(new UnitDataToPack { UnitStatus = (int)UnitStatus.Returned, WashProgramGroupKeyId = 1, UnitKeyId = 1, ProductSerialKeyId = 1 }));
        batchService.Setup(r => r.GetLastBatchProcessesByUnitKeyIdsAndBatchTypeAsync(new List<int> { 1 }, BatchType.PostWash))
          .ReturnsAsync(await Task.FromResult((IList<BatchProcessData>)modifiedBatchesProcessData));
        _serialService.Setup(r => r.GetByKeyIdAsync(args.ProductSerialKeyId))
            .ReturnsAsync(await Task.FromResult(new SerialModel() { SerialNo = "1" }));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainPackErrorCodes.SerialNumberUnitWashBatchNotApproved, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPackValidator.PackValidateAsync")]
    public async Task PackValidateAsync_SerialNumberIsDead_ThrowsException()
    {
        // Arrange
        UnitPackArgs args = new()
        {
            ProductSerialKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };

        _unitPackRepository.Setup(r => r.GetDataToPackAsync(args))
            .ReturnsAsync(await Task.FromResult(new UnitDataToPack { UnitStatus = (int)UnitStatus.Returned, WashProgramGroupKeyId = 1, UnitKeyId = 1, ProductSerialKeyId = 1 }));
        _serialService.Setup(r => r.GetByKeyIdAsync(args.ProductSerialKeyId))
            .ReturnsAsync(await Task.FromResult(new SerialModel { Status = (int)TDocConstants.ObjectStatus.Dead }));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPackValidator.PackValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    #endregion PackValidateAsync
}