using Moq;
using Xunit;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Units.Dispatch;
using ProductionService.Shared.Dtos.Units;
using ProductionService.Core.Services.Units;

namespace ProductionService.Core.Test.Services.Units.Dispatch;

public class UnitDispatchValidatorTests
{
    // Service to test.
    private readonly IUnitDispatchValidator _dispatchUnitValidator;

    // Injected services.
    private readonly Mock<IUnitRepository> _unitRepository;
    private readonly Mock<IUnitValidator> _unitValidator;

    public UnitDispatchValidatorTests()
    {
        _unitRepository = new();
        _unitValidator = new();

        _dispatchUnitValidator = new UnitDispatchValidator(_unitRepository.Object, _unitValidator.Object);
    }

    #region DispatchValidateAsync

    [Fact]
    [Trait("Category", "UnitDispatchValidator.DispatchValidateAsync")]
    public async Task DispatchValidateAsync_KeyIdNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var args = new UnitDispatchArgs
        {
            CustomerKeyId = 1001,
            UnitKeyIds = new List<int>() { 0 },
            LocationKeyId = 5
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _dispatchUnitValidator.DispatchValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitDispatchValidator.DispatchValidateAsync")]
    public async Task DispatchValidateAsync_KeyIdExist_ValidatesSuccessfully()
    {
        // Arrange
        var args = new UnitDispatchArgs
        {
            UnitKeyIds = new List<int>() { 0 }
        };

        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(It.IsAny<int>()))
          .ReturnsAsync(new LocationModel());

        // Act
        var exception = await Record.ExceptionAsync(() => _dispatchUnitValidator.DispatchValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    [Trait("Category", "UnitDispatchValidator.DispatchValidateAsync")]
    public async Task DispatchValidateAsync_DispatchUnitsArgsIsNull_ThrowsException()
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(() => _dispatchUnitValidator.DispatchValidateAsync(null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNull, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitDispatchValidator.DispatchValidateAsync")]
    public async Task DispatchValidateAsync_CustomerOrReturnToStockNotSelected_ThrowsException()
    {
        // Arrange
        var args = new UnitDispatchArgs{ CustomerKeyId = 0 };

        // Act
        var exception = await Record.ExceptionAsync(() => _dispatchUnitValidator.DispatchValidateAsync(args)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainDispatchErrorCodes.CustomerOrReturnToStockNotSelected, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitDispatchValidator.DispatchValidateAsync")]
    public async Task DispatchValidateAsync_LocationKeyIdNotExist_ThrowsException()
    {
        // Arrange
        var args = new UnitDispatchArgs
        {
            CustomerKeyId = 1001,
            LocationKeyId = 10360,
            UnitKeyIds = new List<int>() { 0 }
        };
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(It.IsAny<int>()))
            .ThrowsAsync(new InputArgumentException(GenericErrorCodes.ArgumentsNotValid));
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<CustomerModel>(args.CustomerKeyId))
            .ReturnsAsync(await Task.FromResult(new CustomerModel()));

        // Act
        var exception = await Record.ExceptionAsync(() => _dispatchUnitValidator.DispatchValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitDispatchValidator.DispatchValidateAsync")]
    public async Task DispatchValidateAsync_DispatchUnitsUnitKeyIdsHasNoEntries_ThrowsException()
    {
        // Arrange
        var args = new UnitDispatchArgs
        {
            CustomerKeyId = 1001,
            UnitKeyIds = new List<int>(),
            SerialKeyIds = new List<int?>()
        };
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<CustomerModel>(args.CustomerKeyId))
            .ReturnsAsync(await Task.FromResult(new CustomerModel()));
        // Act
        var exception = await Record.ExceptionAsync(() => _dispatchUnitValidator.DispatchValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.Empty, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitDispatchValidator.DispatchValidateAsync")]
    public async Task DispatchValidateAsync_CustomerKeyIdNotExist_ThrowsException()
    {
        // Arrange
        var args = new UnitDispatchArgs
        {
            CustomerKeyId = 10010,
            LocationKeyId = 1036,
            UnitKeyIds = new List<int>() { 0 }
        };
        var units = new List<UnitModel>() { new() { KeyId = 0, Status = (int)UnitStatus.Stock } };

        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(args.LocationKeyId))
            .ReturnsAsync(await Task.FromResult(new LocationModel()));
        _unitRepository.Setup(r => r.FindByKeyIdAsync(It.IsAny<int>()))
            .ReturnsAsync(units.First());

        // Act
        var exception = await Record.ExceptionAsync(() => _dispatchUnitValidator.DispatchValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitDispatchValidator.DispatchValidateAsync")]
    public async Task DispatchValidateAsync_UnitStatusNotOnStock_ThrowsException()
    {
        // Arrange
        int unitKeyId = 1001;
        var args = new UnitDispatchArgs
        {
            CustomerKeyId = 1001,
            LocationKeyId = 1036,
            UnitKeyIds = new List<int>() { unitKeyId },
            SerialKeyIds = new List<int?>() { 1 }
            
        };
        var units = new List<UnitModel>() { 
            new() {
                KeyId = unitKeyId,
                Status = (int)It.Is<UnitStatus>(s => s != UnitStatus.Stock),
                Seri = new SerialModel { SerialNo = "1"}
            }
        };
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<CustomerModel>(args.CustomerKeyId))
            .ReturnsAsync(await Task.FromResult(new CustomerModel()));
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UnitModel>(It.IsAny<int>()))
            .ReturnsAsync(units.First());
        _unitRepository.Setup(r => r.FindByKeyIdAsync(It.IsAny<int>()))
            .ReturnsAsync(units.First());
        _unitRepository.Setup(r => r.GetWithProductAndItemByKeyIdsAsync(args.UnitKeyIds))
            .ReturnsAsync(await Task.FromResult(new List<UnitModel>()));
        _unitRepository.Setup(r => r.GetWithProductAndItemBySerialKeyIdsAsync(args.SerialKeyIds))
            .ReturnsAsync(await Task.FromResult(units));
       _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(It.IsAny<int>()))
            .ReturnsAsync(new LocationModel());
        _unitRepository.Setup(r => r.FindByKeyIdAsync(It.IsAny<int>()))
            .ReturnsAsync(units.First());

        // Act
        var exception = await Record.ExceptionAsync(() => _dispatchUnitValidator.DispatchValidateAsync(args)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainDispatchErrorCodes.SerialNumberUnitStatusNotOnStock, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitDispatchValidator.DispatchValidateAsync")]
    public async Task DispatchValidateAsync_UnitExpired_ThrowsException()
    {
        // Arrange
        UnitDispatchArgs args = new()
        {
            CustomerKeyId = 1001,
            UnitKeyIds = new List<int> { 1001 },
            LocationKeyId = 1036
        };

        UnitModel unit = new()
        {
            KeyId = args.UnitKeyIds[0],
            Expire = DateTime.Now.AddDays(-1)
        };

        var units = new List<UnitModel>() { unit };

        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<CustomerModel>(args.CustomerKeyId))
           .ReturnsAsync(await Task.FromResult(new CustomerModel()));
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(args.LocationKeyId))
            .ReturnsAsync(new LocationModel());
        _unitRepository.Setup(r => r.FindByKeyIdAsync(It.IsAny<int>()))
            .ReturnsAsync(units.First());
        _unitRepository.Setup(r => r.GetWithProductAndItemByKeyIdsAsync(args.UnitKeyIds))
            .ReturnsAsync(await Task.FromResult(units));

        // Act
        var exception = await Record.ExceptionAsync(() => _dispatchUnitValidator.DispatchValidateAsync(args)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainDispatchErrorCodes.UnitExpired, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitDispatchValidator.DispatchValidateAsync")]
    public async Task DispatchValidateAsync_SerialNumberUnitStatusNotOnStock_ThrowsException()
    {
        // Arrange
        int unitKeyId = 1001;
        var args = new UnitDispatchArgs
        {
            CustomerKeyId = 1001,
            LocationKeyId = 1036,
            UnitKeyIds = new List<int>() { unitKeyId },
            SerialKeyIds = new List<int?>() { 1 }
        };
        var units = new List<UnitModel>() { new()
            {
                KeyId = unitKeyId,
                Status = (int)It.Is<UnitStatus>(s => s != UnitStatus.Stock),
                Seri = new SerialModel { SerialNo = "1" }
            } };
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<CustomerModel>(args.CustomerKeyId))
            .ReturnsAsync(await Task.FromResult(new CustomerModel()));
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(It.IsAny<int>()))
           .ReturnsAsync(new LocationModel());
        _unitRepository.Setup(r => r.FindByKeyIdAsync(It.IsAny<int>()))
            .ReturnsAsync(units.First());
        _unitRepository.Setup(r => r.GetWithProductAndItemByKeyIdsAsync(args.UnitKeyIds))
            .ReturnsAsync(await Task.FromResult(new List<UnitModel>()));
        _unitRepository.Setup(r => r.GetWithProductAndItemBySerialKeyIdsAsync(args.SerialKeyIds))
            .ReturnsAsync(await Task.FromResult(units));
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(args.LocationKeyId))
            .ReturnsAsync(await Task.FromResult(new LocationModel()));

        // Act
        var exception = await Record.ExceptionAsync(() => _dispatchUnitValidator.DispatchValidateAsync(args)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainDispatchErrorCodes.SerialNumberUnitStatusNotOnStock, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitDispatchValidator.DispatchValidateAsync")]
    public async Task DispatchValidateAsync_SerialNumberUnitExpired_ThrowsException()
    {
        // Arrange
        int unitKeyId = 1001;
        UnitDispatchArgs args = new()
        {
            CustomerKeyId = 1001,
            LocationKeyId = 1036,
            UnitKeyIds = new List<int> { unitKeyId },
            SerialKeyIds = new List<int?> { 1 }
        };

        UnitModel unit = new()
        {
            KeyId = unitKeyId,
            Expire = DateTime.Now.AddDays(-1),
            Seri = new SerialModel { SerialNo = "1" }
        };

        var units = new List<UnitModel>() { unit };

        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<CustomerModel>(args.CustomerKeyId))
            .ReturnsAsync(await Task.FromResult(new CustomerModel()));
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(args.LocationKeyId))
           .ReturnsAsync(new LocationModel());
        _unitRepository.Setup(r => r.FindByKeyIdAsync(It.IsAny<int>()))
            .ReturnsAsync(unit);
        _unitRepository.Setup(r => r.GetWithProductAndItemByKeyIdsAsync(args.UnitKeyIds))
            .ReturnsAsync(await Task.FromResult(new List<UnitModel>()));
        _unitRepository.Setup(r => r.GetWithProductAndItemBySerialKeyIdsAsync(args.SerialKeyIds))
            .ReturnsAsync(await Task.FromResult(units));
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(args.LocationKeyId))
            .ReturnsAsync(await Task.FromResult(new LocationModel()));

        // Act
        var exception = await Record.ExceptionAsync(() => _dispatchUnitValidator.DispatchValidateAsync(args)) as DomainException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(DomainDispatchErrorCodes.SerialNumberUnitExpired, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitDispatchValidator.DispatchValidateAsync")]
    public async Task DispatchValidateAsync_DispatchUnitsArgsAreValid_NotThrowsException()
    {
        // Arrange
        int unitKeyId = 1001;
        var args = new UnitDispatchArgs()
        {
            LocationKeyId = 1036,
            CustomerKeyId = 1001,
            UnitKeyIds = new List<int> { unitKeyId }
        };
        var units = new List<UnitModel>() { new() { KeyId = unitKeyId, Status = (int)UnitStatus.Stock } };

        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<CustomerModel>(args.CustomerKeyId))
           .ReturnsAsync(await Task.FromResult(new CustomerModel()));
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(It.IsAny<int>()))
            .ReturnsAsync(new LocationModel());
        _unitRepository.Setup(r => r.FindByKeyIdAsync(It.IsAny<int>()))
            .ReturnsAsync(units.First());
        _unitRepository.Setup(r => r.GetWithProductAndItemByKeyIdsAsync(args.UnitKeyIds))
            .ReturnsAsync(await Task.FromResult(units));
        _unitRepository.Setup(r => r.GetWithProductAndItemBySerialKeyIdsAsync(args.SerialKeyIds))
           .ReturnsAsync(await Task.FromResult(new List<UnitModel>()));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _dispatchUnitValidator.DispatchValidateAsync(args));

        // Assert
        Assert.Null(exception);
    }
    
    #endregion
}