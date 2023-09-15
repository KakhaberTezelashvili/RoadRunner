using Moq;
using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Core.Services.Units.Lots;
using ProductionService.Shared.Dtos.Lots;
using ProductionService.Shared.Dtos.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Lots;

public class UnitLotValidatorTests
{
    private const int _unitKeyId = 1;
    private const int _userKeyId = 1;
    private const int _locationKeyId = 1;
    private readonly List<UnitLotInformationArgs> _lotInfoList;

    // Service to test.
    private readonly IUnitLotValidator _unitLotValidator;

    // Injected services.
    private readonly Mock<IUnitLotRepository> _unitLotRepository;

    public UnitLotValidatorTests()
    {
        _lotInfoList = new List<UnitLotInformationArgs>
        {
            new() { KeyId = 1, Position = 1 },
            new() { KeyId = 2, Position = 2 }
        };

        _unitLotRepository = new();
        _unitLotValidator = new UnitLotValidator(_unitLotRepository.Object);
    }

    #region UpdateLotsValidateAsync

    [Fact]
    [Trait("Category", "UnitLotValidator.UpdateLotsValidateAsync")]
    public async Task UpdateLotsValidateAsync_UnitLotInformationUpdateArgsIsNull_ThrowsException()
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(()  => _unitLotValidator.UpdateLotsValidateAsync(null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNull, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitLotValidator.UpdateLotsValidateAsync")]
    public async Task UpdateLotsValidateAsync_ArgsLocationKeyIdIsZero_ThrowsException()
    {
        // Arrange
        UnitLotsArgs args = new()
        {
            LotInformationList = _lotInfoList,
            UnitKeyId = _unitKeyId,
            UserKeyId = _userKeyId
        };
        _unitLotRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UnitModel>(It.IsAny<int>()))
          .ReturnsAsync(new UnitModel());

        // Act
        var exception = await Record.ExceptionAsync(() => _unitLotValidator.UpdateLotsValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitLotValidator.UpdateLotsValidateAsync")]
    public async Task UpdateLotsValidateAsync_ArgsLotInfoListIsEmpty_ThrowsException()
    {
        // Arrange
        UnitLotsArgs args = new()
        {
            LocationKeyId = _locationKeyId,
            UnitKeyId = _unitKeyId,
            UserKeyId = _userKeyId
        };
        _unitLotRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UnitModel>(It.IsAny<int>()))
          .ReturnsAsync(new UnitModel());
        _unitLotRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(It.IsAny<int>()))
          .ReturnsAsync(new LocationModel());
        _unitLotRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(It.IsAny<int>()))
          .ReturnsAsync(new UserModel());

        // Act
        var exception = await Record.ExceptionAsync(() => _unitLotValidator.UpdateLotsValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitLotValidator.UpdateLotsValidateAsync")]
    public async Task UpdateLotsValidateAsync_ArgsUnitKeyIdIsZero_ThrowsException()
    {
        // Arrange
        UnitLotsArgs args = new()
        {
            LocationKeyId = _locationKeyId,
            LotInformationList = _lotInfoList,
            UserKeyId = _userKeyId
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _unitLotValidator.UpdateLotsValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitLotValidator.UpdateLotsValidateAsync")]
    public async Task UpdateLotsValidateAsync_ArgsUserKeyIdIsZero_ThrowsException()
    {
        // Arrange
        UnitLotsArgs args = new()
        {
            LocationKeyId = _locationKeyId,
            LotInformationList = _lotInfoList,
            UnitKeyId = _unitKeyId
        };
        _unitLotRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UnitModel>(It.IsAny<int>()))
          .ReturnsAsync(new UnitModel());
        _unitLotRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(It.IsAny<int>()))
          .ReturnsAsync(new LocationModel());

        // Act
        var exception = await Record.ExceptionAsync(() => _unitLotValidator.UpdateLotsValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitLotValidator.UpdateLotsValidateAsync")]
    public async Task UpdateLotsValidateAsync_UnitLotInformationUpdateArgsIsValid_NotThrowsException()
    {
        // Arrange
        UnitLotsArgs args = new()
        {
            LocationKeyId = _locationKeyId,
            LotInformationList = _lotInfoList,
            UnitKeyId = _unitKeyId,
            UserKeyId = _userKeyId
        };
        _unitLotRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UnitModel>(It.IsAny<int>()))
          .ReturnsAsync(new UnitModel());
        _unitLotRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<LocationModel>(It.IsAny<int>()))
          .ReturnsAsync(new LocationModel());
        _unitLotRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<UserModel>(It.IsAny<int>()))
          .ReturnsAsync(new UserModel());

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitLotValidator.UpdateLotsValidateAsync(args));

        // Assert
        Assert.Null(exception);
    }

    #endregion UpdateLotsValidateAsync
}