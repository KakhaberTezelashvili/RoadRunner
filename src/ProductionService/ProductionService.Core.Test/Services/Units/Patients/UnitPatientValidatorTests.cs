using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Units.Patients;
using ProductionService.Shared.Dtos.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Patients;

public class UnitPatientValidatorTests
{
    private const int _unitKeyId = 1;
    private const int _userKeyId = 1;

    // Service to test.
    private readonly IUnitPatientValidator _unitPatientValidator;

    // Injected services.
    private readonly Mock<IUnitRepository> _unitRepository;

    public UnitPatientValidatorTests()
    {
        _unitRepository = new();
        _unitPatientValidator = new UnitPatientValidator(_unitRepository.Object);
    }

    #region UpdatePatientValidateAsync

    [Fact]
    [Trait("Category", "UnitPatientValidator.UpdatePatientValidateAsync")]
    public async Task UpdatePatientValidateAsync_PatientUnitArgsIsNull_ThrowsException()
    {
        // Arrange

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPatientValidator.UpdatePatientValidateAsync(0, 0, null)) as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNull, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPatientValidator.UpdatePatientValidateAsync")]
    public async Task UpdatePatientValidateAsync_UserKeyIdAndUnitKeyIdAreZero_ThrowsException()
    {
        // Arrange
        UnitPatientArgs args = new()
        {
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPatientValidator.UpdatePatientValidateAsync(0, 0, args))
            as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPatientValidator.UpdatePatientValidateAsync")]
    public async Task UpdatePatientValidateAsync_PatientKeyIdNotZeroAndLocationKeyIdAndPositionLocationKeyIdAndFactoryKeyIdAreZero_ThrowsException()
    {
        // Arrange
        UnitPatientArgs args = new()
        {
            PatientKeyId = 1,
            LocationKeyId = 0,
            FactoryKeyId = 0,
            PositionLocationKeyId = 0
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPatientValidator.UpdatePatientValidateAsync(_unitKeyId, _userKeyId, args))
            as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPatientValidator.UpdatePatientValidateAsync")]
    public async Task UpdatePatientValidateAsync_UserKeyIdAndUnitKeyIdAreNotZeroAndPatientKeyIdIsZero_NotThrowsException()
    {
        // Arrange
        UnitPatientArgs args = new()
        {
            PatientKeyId = 0,
            LocationKeyId = 0,
            FactoryKeyId = 0,
            PositionLocationKeyId = 0
        };
        _unitRepository.Setup(r => r.FindByKeyIdAsync(_unitKeyId))
           .ReturnsAsync(await Task.FromResult(new UnitModel { Status = (int)UnitStatus.Returned }));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitPatientValidator.UpdatePatientValidateAsync(_unitKeyId, _userKeyId, args));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    [Trait("Category", "UnitPatientValidator.UpdatePatientValidateAsync")]
    public async Task UpdatePatientValidateAsync_PatientKeyIdAndLocationKeyIdAndPositionLocationKeyIdAndFactoryKeyIdAreZero_NotThrowInputArgumentException()
    {
        // Arrange
        UnitPatientArgs args = new()
        {
            PatientKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPatientValidator.UpdatePatientValidateAsync(_unitKeyId, _userKeyId, args))
            as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.NotEqual(GenericErrorCodes.ArgumentsNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPatientValidator.UpdatePatientValidateAsync")]
    public async Task UpdatePatientValidateAsync_UnitNotFound_ThrowsException()
    {
        // Arrange
        UnitPatientArgs args = new()
        {
            PatientKeyId = 0,
            LocationKeyId = 0,
            FactoryKeyId = 0,
            PositionLocationKeyId = 0
        };
        _unitRepository.Setup(r => r.GetUnitAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult((UnitModel)null));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPatientValidator.UpdatePatientValidateAsync(_unitKeyId, _userKeyId, args))
            as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPatientValidator.UpdatePatientValidateAsync")]
    public async Task UpdatePatientValidateAsync_UnitStatusIsNotPacked_ThrowsException()
    {
        // Arrange
        UnitPatientArgs args = new()
        {
            PatientKeyId = 0,
            LocationKeyId = 0,
            FactoryKeyId = 0,
            PositionLocationKeyId = 0
        };
        _unitRepository.Setup(r => r.FindByKeyIdAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult(new UnitModel { Status = (int)UnitStatus.Init }));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPatientValidator.UpdatePatientValidateAsync(_unitKeyId, _unitKeyId, args))
            as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(InputArgumentPatientErrorCodes.PatientStatusNotValid, exception.Code);
    }

    [Fact]
    [Trait("Category", "UnitPatientValidator.UpdatePatientValidateAsync")]
    public async Task UpdatePatientValidateAsync_PatientKeyIdNotZeroAndPatientNotFound_ThrowsException()
    {
        // Arrange
        UnitPatientArgs args = new()
        {
            PatientKeyId = 1,
            LocationKeyId = 1,
            FactoryKeyId = 1,
            PositionLocationKeyId = 1
        };
        _unitRepository.Setup(r => r.GetUnitAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult(new UnitModel { Status = (int)UnitStatus.Returned }));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitPatientValidator.UpdatePatientValidateAsync(_unitKeyId, _userKeyId, args))
            as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    #endregion UpdatePatientValidateAsync
}