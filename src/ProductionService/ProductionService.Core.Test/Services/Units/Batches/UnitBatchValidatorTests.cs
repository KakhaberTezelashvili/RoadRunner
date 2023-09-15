using Microsoft.Extensions.DependencyInjection;
using Moq;
using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Core.Services.Batches;
using ProductionService.Core.Services.Units.Batches;
using ProductionService.Shared.Dtos.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Batches;

public class UnitBatchValidatorTests
{
    // Service to test.
    private readonly IUnitBatchValidator _unitBatchValidator;

    // Injected services.
    private readonly Mock<IUnitBatchRepository> _unitBatchRepository;
    private readonly Mock<IServiceProvider> _serviceProvider;
    public UnitBatchValidatorTests()
    {
        _unitBatchRepository = new();
        _serviceProvider = new();
        Mock<IServiceScope> serviceScope = new();
        serviceScope.Setup(s => s.ServiceProvider)
            .Returns(_serviceProvider.Object);

        Mock<IServiceScopeFactory> serviceScopeFactory = new();
        serviceScopeFactory.Setup(x => x.CreateScope())
            .Returns(serviceScope.Object);

        _serviceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);
        _unitBatchValidator = new UnitBatchValidator(_unitBatchRepository.Object, _serviceProvider.Object);
    }

    #region GetBatchContentsValidateAsync

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_UnitIdListAreEmpty_ThrowsException()
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(WhatType.SteriPreBatch, null, new List<int>()))
            as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.Empty, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_UnitListFromDatabaseAreEmpty_ThrowsException()
    {
        // Arrange
        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(WhatType.Pack, 0, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(new List<UnitBatchContentsDto>()));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(WhatType.SteriPreBatch, null, new List<int>()))
            as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.Empty, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_UnitIdsAreNotEqualWithUnitList_ThrowsException()
    {
        // Arrange
        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(It.IsAny<WhatType>(), null, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(new List<UnitBatchContentsDto> { new() }));

        Mock<IBatchValidator> batchValidator = new();
        batchValidator.Setup(r => r.AssignUnitsToSterilizeOrWashBatchValidateAsync(It.IsAny<MachineType>(), It.IsAny<List<int>>(), It.IsAny<bool>()))
            .Returns(Task.CompletedTask);

        _serviceProvider.Setup(x => x.GetService(typeof(IBatchValidator)))
            .Returns(batchValidator.Object);

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(WhatType.WashPreBatch, null, new List<int> { 1, 2, 3, 4 }))
            as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(InputArgumentUnitErrorCodes.UnitListMissingFound, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_UnitStatusNotPacked_ThrowsException()
    {
        // Arrange
        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(WhatType.SteriPreBatch, null, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(UnitBatchTestsData.InvalidUnitBasicBatchRegistrations));

        Mock<IBatchValidator> batchValidator = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchValidator)))
            .Returns(batchValidator.Object);

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(
            WhatType.SteriPreBatch, null, UnitBatchTestsData.UnitKeyIds)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainBatchErrorCodes.UnitStatusNotPacked, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_SerialNumberUnitStatusNotPacked_ThrowsException()
    {
        // Arrange
        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(WhatType.SteriPreBatch, null, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(UnitBatchTestsData.InvalidUnitBasicBatchRegistrations));

        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.SerialKeyId1))
            .ReturnsAsync(await Task.FromResult(new SerialModel()
            {
                KeyId = UnitBatchTestsData.SerialKeyId1,
                UnitUnit = UnitBatchTestsData.UnitKeyId1
            }
            ));

        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.SerialKeyId2))
            .ReturnsAsync(await Task.FromResult(new SerialModel()
            {
                KeyId = UnitBatchTestsData.SerialKeyId2,
                UnitUnit = UnitBatchTestsData.UnitKeyId2
            }));

        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.SerialKeyId3))
            .ReturnsAsync(await Task.FromResult(new SerialModel()
            {
                KeyId = UnitBatchTestsData.SerialKeyId3,
                UnitUnit = UnitBatchTestsData.UnitKeyId3
            }));

        Mock<IBatchValidator> batchValidator = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchValidator)))
            .Returns(batchValidator.Object);

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(
            WhatType.SteriPreBatch, null, null, UnitBatchTestsData.SerialKeyIds)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainBatchErrorCodes.SerialNumberUnitStatusNotPacked, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_UnitStatusNotReturned_ThrowsException()
    {
        // Arrange
        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(WhatType.WashPreBatch, null, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(UnitBatchTestsData.InvalidUnitBasicBatchRegistrations));

        Mock<IBatchValidator> batchValidator = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchValidator)))
            .Returns(batchValidator.Object);

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(
            WhatType.WashPreBatch, null, UnitBatchTestsData.UnitKeyIds)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainBatchErrorCodes.UnitStatusNotReturned, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_SerialNumberUnitStatusNotReturned_ThrowsException()
    {
        // Arrange
        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(WhatType.WashPreBatch, null, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(UnitBatchTestsData.InvalidUnitBasicBatchRegistrations));

        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.SerialKeyId1))
            .ReturnsAsync(await Task.FromResult(new SerialModel()
            {
                KeyId = UnitBatchTestsData.SerialKeyId1,
                UnitUnit = UnitBatchTestsData.UnitKeyId1
            }
            ));

        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.SerialKeyId2))
            .ReturnsAsync(await Task.FromResult(new SerialModel()
            {
                KeyId = UnitBatchTestsData.SerialKeyId2,
                UnitUnit = UnitBatchTestsData.UnitKeyId2
            }));

        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.SerialKeyId3))
            .ReturnsAsync(await Task.FromResult(new SerialModel()
            {
                KeyId = UnitBatchTestsData.SerialKeyId3,
                UnitUnit = UnitBatchTestsData.UnitKeyId3
            }));

        Mock<IBatchValidator> batchValidator = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchValidator)))
            .Returns(batchValidator.Object);

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(
            WhatType.WashPreBatch, null, null, UnitBatchTestsData.SerialKeyIds)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainBatchErrorCodes.SerialNumberUnitStatusNotReturned, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_UnitListHaveInvalidWhatTypeForWasher_ThrowsException()
    {
        // Arrange
        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(WhatType.WashPreBatch, null, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(UnitBatchTestsData.InvalidUnitBasicBatchRegistrations));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(
            WhatType.Cancel, null, UnitBatchTestsData.UnitKeyIds)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(InputArgumentUnitErrorCodes.WhatTypeNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_WhatTypeHasWrongValue_ThrowsException()
    {
        // Arrange
        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(WhatType.SteriPreBatch, null, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(UnitBatchTestsData.ValidUnitBasicSterilizerBatchRegistrations));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(
            WhatType.Cancel, null, UnitBatchTestsData.UnitKeyIds)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(InputArgumentUnitErrorCodes.WhatTypeNotValid, exception.Code);
    }

    [Theory]
    [InlineData(WhatType.SteriPreBatch)]
    [InlineData(WhatType.WashPreBatch)]
    [InlineData(WhatType.Out)]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_DataIsValid_NotThrowsException(WhatType whatType)
    {
        // Arrange
        var testData = new List<UnitBatchContentsDto>();
        switch (whatType)
        {
            case WhatType.SteriPreBatch:
                testData.AddRange(UnitBatchTestsData.ValidUnitBasicSterilizerBatchRegistrations);
                break;

            case WhatType.WashPreBatch:
                testData.AddRange(UnitBatchTestsData.ValidUnitBasicWasherBatchRegistrations);
                break;

            case WhatType.Out:
                testData.AddRange(UnitBatchTestsData.ValidUnitBasicStockBatchRegistrations);
                break;
        }

        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(whatType, null, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(
                testData));

        Mock<IBatchValidator> batchValidator = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchValidator)))
            .Returns(batchValidator.Object);

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(
            whatType, null, UnitBatchTestsData.UnitKeyIds));

        // Assert
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(WhatType.SteriPreBatch)]
    [InlineData(WhatType.WashPreBatch)]
    [InlineData(WhatType.Out)]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_DataIsValidWithSerial_NotThrowsException(WhatType whatType)
    {
        // Arrange
        var testData = new List<UnitBatchContentsDto>();
        switch (whatType)
        {
            case WhatType.SteriPreBatch:
                testData.AddRange(UnitBatchTestsData.ValidUnitBasicSterilizerBatchRegistrations);
                break;

            case WhatType.WashPreBatch:
                testData.AddRange(UnitBatchTestsData.ValidUnitBasicWasherBatchRegistrations);
                break;

            case WhatType.Out:
                testData.AddRange(UnitBatchTestsData.ValidUnitBasicStockBatchRegistrations);
                break;
        }

        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(whatType, null, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(
                testData));

        Mock<IBatchValidator> batchValidator = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchValidator)))
            .Returns(batchValidator.Object);

        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.SerialKeyId1))
            .ReturnsAsync(await Task.FromResult(new SerialModel { UnitUnit = UnitBatchTestsData.UnitKeyId1 }));

        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.UnitKeyId2))
            .ReturnsAsync(await Task.FromResult(new SerialModel { UnitUnit = UnitBatchTestsData.UnitKeyId2 }));

        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.UnitKeyId3))
            .ReturnsAsync(await Task.FromResult(new SerialModel { UnitUnit = UnitBatchTestsData.UnitKeyId3 }));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(
            whatType, null, null, UnitBatchTestsData.SerialKeyIds));

        // Assert
        Assert.Null(exception);
    }

    [Theory]
    [InlineData(WhatType.SteriPreBatch)]
    [InlineData(WhatType.WashPreBatch)]
    [InlineData(WhatType.Out)]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_DataIsValidWithSerialNotFound_ThrowsException(WhatType whatType)
    {
        // Arrange
        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.SerialKeyId1))
            .ReturnsAsync(await Task.FromResult((SerialModel)null));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(
            whatType, null, null, UnitBatchTestsData.SerialKeyIds)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_BatchIdIsZero_ThrowsException()
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(WhatType.SteriPreBatch, 0, new List<int>()))
            as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_DispatchUnitExpired_ThrowsException()
    {
        // Arrange
        WhatType whatType = WhatType.Out;
        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(whatType, null, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(
                new List<UnitBatchContentsDto>
                {
                    new() { ExpiryDate = DateTime.Now.AddDays(-1) }
                }));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(
            whatType, null, UnitBatchTestsData.UnitKeyIds)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainDispatchErrorCodes.UnitExpired, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_DispatchSerialNumberExpired_ThrowsException()
    {
        // Arrange
        WhatType whatType = WhatType.Out;
        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(whatType, null, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(
                new List<UnitBatchContentsDto>
                {
                    new() { ExpiryDate = DateTime.Now.AddDays(-1) }
                }));
        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.SerialKeyId1))
            .ReturnsAsync(await Task.FromResult(new SerialModel()
            {
                KeyId = UnitBatchTestsData.SerialKeyId1,
                UnitUnit = UnitBatchTestsData.UnitKeyId1
            }));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(
            whatType, null, null, new List<int>() { 1 })) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainDispatchErrorCodes.SerialNumberUnitExpired, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_UnitStatusNotOnStock_ThrowsException()
    {
        // Arrange
        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(WhatType.Out, null, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(UnitBatchTestsData.InvalidUnitBasicBatchRegistrations));

        Mock<IBatchValidator> batchValidator = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchValidator)))
            .Returns(batchValidator.Object);

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(
            WhatType.Out, null, UnitBatchTestsData.UnitKeyIds)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainDispatchErrorCodes.UnitStatusNotOnStock, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitBatchValidator.GetBatchContentsValidateAsync")]
    public async Task GetBatchContentsValidateAsync_SerialNumberUnitStatusNotOnStock_ThrowsException()
    {
        // Arrange
        _unitBatchRepository.Setup(r => r.GetBatchContentsAsync(WhatType.Out, null, It.IsAny<List<int>>()))
            .ReturnsAsync(await Task.FromResult(UnitBatchTestsData.InvalidUnitBasicBatchRegistrations));

        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.SerialKeyId1))
            .ReturnsAsync(await Task.FromResult(new SerialModel()
            {
                KeyId = UnitBatchTestsData.SerialKeyId1,
                UnitUnit = UnitBatchTestsData.UnitKeyId1
            }));

        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.SerialKeyId2))
            .ReturnsAsync(await Task.FromResult(new SerialModel()
            {
                KeyId = UnitBatchTestsData.SerialKeyId2,
                UnitUnit = UnitBatchTestsData.UnitKeyId2
            }));

        _unitBatchRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<SerialModel>(UnitBatchTestsData.SerialKeyId3))
             .ReturnsAsync(await Task.FromResult(new SerialModel()
             {
                 KeyId = UnitBatchTestsData.SerialKeyId3,
                 UnitUnit = UnitBatchTestsData.UnitKeyId3
             }));

        Mock<IBatchValidator> batchValidator = new();
        _serviceProvider.Setup(x => x.GetService(typeof(IBatchValidator)))
            .Returns(batchValidator.Object);

        // Act
        var exception = await Record.ExceptionAsync(() => _unitBatchValidator.GetBatchContentsValidateAsync(
            WhatType.Out, null, null, UnitBatchTestsData.SerialKeyIds)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainDispatchErrorCodes.SerialNumberUnitStatusNotOnStock, exception.Code);
    }

    #endregion GetBatchContentsValidateAsync
}