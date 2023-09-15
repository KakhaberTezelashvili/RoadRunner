using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Texts;
using ProductionService.Core.Services.Units.Errors;
using ProductionService.Shared.Dtos.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Errors;

public class UnitErrorValidatorTests
{
    // Service to test.
    private readonly IUnitErrorValidator _unitErrorValidator;

    // Injected services.
    private readonly Mock<ITextService> _textService;
    private readonly Mock<IUnitRepository> _unitRepository;

    public UnitErrorValidatorTests()
    {
        _textService = new();
        _unitRepository = new();
        _unitErrorValidator = new UnitErrorValidator(_unitRepository.Object, _textService.Object);
    }

    #region UpdateErrorValidateAsync

    [Fact]
    [Trait("Category", "UnitErrorValidator.UpdateErrorValidateAsync")]
    public async Task UpdateErrorValidateAsync_ErrorCodeNotFound_ThrowsException()
    {
        // Arrange
        int unitKeyId = 1;
        UnitErrorArgs errorUnitArgs = new() { ErrorNumber = 0 };

        _unitRepository.Setup(r => r.FindByKeyIdAsync(It.IsAny<int>()))
            .ReturnsAsync(await Task.FromResult(new UnitModel()));
        _textService.Setup(r => r.GetErrorAsync(It.IsAny<int>()))
            .ReturnsAsync(await Task.FromResult((TextModel)null));

        // Act
        var exception = await Record.ExceptionAsync(() => _unitErrorValidator.UpdateErrorValidateAsync(unitKeyId, errorUnitArgs))
            as InputArgumentException;

        // Assert
        Assert.NotNull(exception);
        Assert.Equal(GenericErrorCodes.NotFound, exception.Code);
    }

    #endregion UpdateErrorValidateAsync
}