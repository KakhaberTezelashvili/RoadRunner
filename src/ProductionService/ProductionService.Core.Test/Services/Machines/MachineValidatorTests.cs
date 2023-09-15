using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Machines;
using ProductionService.Shared.Dtos.Machines;
using Xunit;

namespace ProductionService.Core.Test.Services.Machines;

public class MachineValidatorTests
{
    private const int _machineKeyId = 1;
    private const int _locationKeyId = 1;

    // Service to test.
    private readonly IMachineValidator _machineValidator;

    // Injected services.
    private readonly Mock<IMachineRepository> _machineRepository;

    public MachineValidatorTests()
    {
        _machineRepository = new();
        _machineValidator = new MachineValidator(_machineRepository.Object);
    }

    #region MachineValidate

    [Fact]
    [Trait("Category", "MachineValidator.MachineValidate")]
    public async void MachineValidate_MachineKeyIdIsZero_ThrowsException()
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(async () => await _machineValidator.FindByKeyIdValidateAsync(0)) as InputArgumentException;

        //Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "MachineValidator.MachineValidate")]
    public async void MachineValidate_MachineKeyIdIsNotZero_NotThrowsException()
    {
        // Arrange
        _machineRepository.Setup(r => r.FindByKeyIdAsync(_machineKeyId)).ReturnsAsync(new MachineModel());

        // Act
        Exception exception = await Record.ExceptionAsync(async () => await _machineValidator.FindByKeyIdValidateAsync(_machineKeyId));

        //Assert
        Assert.Null(exception);
    }

    #endregion MachineValidate

    #region LocationValidate

    [Fact]
    [Trait("Category", "MachineValidator.LocationValidate")]
    public async void LocationValidate_LocationKeyIdIsZero_ThrowsException()
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(async () => await _machineValidator.FindOtherEntityByKeyIdValidateAsync<LocationModel>(0)) as InputArgumentException;

        //Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "MachineValidator.LocationValidate")]
    public async void LocationValidate_LocationKeyIdIsNotZero_NotThrowsException()
    {
        // Arrange
        _machineRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(_locationKeyId))
            .ReturnsAsync(new LocationModel());

        // Act
        Exception exception = await Record.ExceptionAsync(async () => await _machineValidator.FindOtherEntityByKeyIdValidateAsync<LocationModel>(_locationKeyId));

        //Assert
        Assert.Null(exception);
    }

    #endregion LocationValidate

    #region MachineInfoForBatchCreatingValidateAsync

    [Fact]
    [Trait("Category", "MachineValidator.MachineInfoForBatchCreatingValidateAsync")]
    public async Task MachineInfoForBatchCreatingValidateAsync_MachineKeyIdIsZero_ThrowsException()
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(() => _machineValidator.MachineInfoForBatchCreatingValidateAsync(0, _locationKeyId)) as InputArgumentException;

        //Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "MachineValidator.MachineInfoForBatchCreatingValidateAsync")]
    public async Task MachineInfoForBatchCreatingValidateAsync_LocationKeyIdIsZero_ThrowsException()
    {
        // Arrange
        _machineRepository.Setup(r => r.FindByKeyIdAsync(_machineKeyId)).ReturnsAsync(new MachineModel());

        // Act
        var exception = await Record.ExceptionAsync(() => _machineValidator.MachineInfoForBatchCreatingValidateAsync(_machineKeyId, 0)) as InputArgumentException;

        //Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "MachineValidator.MachineInfoForBatchCreatingValidateAsync")]
    public async Task MachineInfoForBatchCreatingValidateAsync_MachineNotFound_ThrowsException()
    {
        // Arrange
        _machineRepository.Setup(r => r.GetDetailsByKeyIdAsync(_machineKeyId))
            .ReturnsAsync((MachineDetailsDto)null);

        // Act
        var exception = await Record.ExceptionAsync(() => _machineValidator.MachineInfoForBatchCreatingValidateAsync(_machineKeyId, _locationKeyId)) as InputArgumentException;

        //Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    [Fact]
    [Trait("Category", "MachineValidator.MachineInfoForBatchCreatingValidateAsync")]
    public async Task MachineInfoForBatchCreatingValidateAsync_MachineNotAvailableAtLocation_ThrowsException()
    {
        // Arrange
        _machineRepository.Setup(r => r.FindByKeyIdAsync(_machineKeyId))
            .ReturnsAsync(new MachineModel());
        _machineRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(_locationKeyId))
            .ReturnsAsync(new LocationModel());
        _machineRepository.Setup(r => r.GetDetailsByKeyIdAsync(_machineKeyId))
            .ReturnsAsync(new MachineDetailsDto { LocationKeyId = 2 });

        // Act
        var exception = await Record.ExceptionAsync(() => _machineValidator.MachineInfoForBatchCreatingValidateAsync(_machineKeyId, _locationKeyId)) as InputArgumentException;

        //Assert
        Assert.NotNull(exception);
        Assert.Equal(InputArgumentMachineErrorCodes.MachineNotAvailableAtLocation, exception.Code);
    }

    [Fact]
    [Trait("Category", "MachineValidator.MachineInfoForBatchCreatingValidateAsync")]
    public async Task MachineInfoForBatchCreatingValidateAsync_MachineAvailableAtLocation_NotThrowsException()
    {
        // Arrange
        _machineRepository.Setup(r => r.FindByKeyIdAsync(_machineKeyId))
            .ReturnsAsync(new MachineModel());
        _machineRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(_locationKeyId))
            .ReturnsAsync(new LocationModel());
        _machineRepository.Setup(r => r.GetDetailsByKeyIdAsync(_machineKeyId))
            .ReturnsAsync(new MachineDetailsDto { LocationKeyId = _locationKeyId });

        // Act
        Exception exception = await Record.ExceptionAsync(() => _machineValidator.MachineInfoForBatchCreatingValidateAsync(_machineKeyId, _locationKeyId));

        //Assert
        Assert.Null(exception);
    }

    #endregion MachineInfoForBatchCreatingValidateAsync
}