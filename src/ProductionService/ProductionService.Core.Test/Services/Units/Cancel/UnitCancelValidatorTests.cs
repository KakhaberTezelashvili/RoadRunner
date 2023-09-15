using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Units;
using ProductionService.Core.Services.Units.Cancel;
using ProductionService.Shared.Dtos.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Cancel;

public class UnitCancelValidatorTests
{
    // Service to test.
    private readonly IUnitCancelValidator _unitCancelValidator;

    // Injected services.
    private readonly Mock<IUnitRepository> _unitRepository;
    private readonly Mock<IUnitValidator> _unitValidator;

    public UnitCancelValidatorTests()
    {
        _unitRepository = new();
        _unitValidator = new();
        _unitCancelValidator = new UnitCancelValidator(_unitRepository.Object, _unitValidator.Object);
    }

    #region CancelValidateAsync

    [Fact]
    [Trait("Category", "UnitCancelValidator.CancelValidateAsync")]
    public async Task CancelValidateAsync_CancelUnitArgsIsNull_ThrowsException()
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(() => _unitCancelValidator.CancelValidateAsync(null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNull, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitCancelValidator.CancelValidateAsync")]
    public async Task CancelValidateAsync_ArgsUnitKeyIdsIsNull_ThrowsException()
    {
        // Arrange
        UnitCancelArgs args = new();

        // Act
        var exception = await Record.ExceptionAsync(() => _unitCancelValidator.CancelValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNull, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitCancelValidator.CancelValidateAsync")]
    public async Task CancelValidateAsync_ArgsUnitKeyIdsContainsZero_ThrowsException()
    {
        // Arrange
        var args = new UnitCancelArgs() { UnitKeyIds = new List<int>() { 0 } };

        // Act
        var exception = await Record.ExceptionAsync(() => _unitCancelValidator.CancelValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitCancelValidator.CancelValidateAsync")]
    public async Task CancelValidateAsync_UnitKeyIdsContainsNotZeroAndUnitsNotFound_ThrowsException()
    {
        // Arrange
        const int unitKeyId = 1;
        UnitCancelArgs args = new()
        {
            UnitKeyIds = new List<int>() { unitKeyId }
        };
        _unitValidator.Setup(r => r.KeyIdsValidateAsync(new List<int>() { unitKeyId }))
            .ThrowsAsync(new InputArgumentException(GenericErrorCodes.NotFound));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitCancelValidator.CancelValidateAsync(args)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitCancelValidator.CancelValidateAsync")]
    public async Task CancelValidateAsync_UnitKeyIdsContainsNotZeroAndUnitFound_NotThrowsException()
    {
        // Arrange
        const int unitKeyId = 1;
        var units = new List<UnitModel>() { new UnitModel() { KeyId = unitKeyId } };
        UnitCancelArgs args = new()
        {
            UnitKeyIds = new List<int>() { unitKeyId }
        };
        _unitRepository.Setup(r => r.FindByKeyIdAsync(unitKeyId))
          .ReturnsAsync(await Task.FromResult(units.First()));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitCancelValidator.CancelValidateAsync(args));

        // Assert
        Assert.Null(exception);
    }

    #endregion CancelValidateAsync
}