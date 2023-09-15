using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Texts;
using ProductionService.Core.Services.Units.Errors;
using ProductionService.Shared.Dtos.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Errors;

public class UnitErrorServiceTests
{
    private const int _unitKeyId = 1001;
    private const int _errorNo = 1001;

    // Service to test.
    private readonly UnitErrorService _unitErrorService;

    // Injected services.
    private readonly Mock<IUnitRepository> _unitRepository;
    private readonly Mock<IUnitErrorValidator> _unitErrorValidator;
    private readonly Mock<ITextService> _textService;

    public UnitErrorServiceTests()
    {
        _unitRepository = new();
        _unitErrorValidator = new();
        _textService = new();
        _unitErrorService = new UnitErrorService(_unitErrorValidator.Object, _unitRepository.Object);
    }

    #region UpdateUnitErrorAsync

    [Fact]
    public async Task UpdateUnitErrorAsync_ReturnsFailedValidateBeforeSetUnitError()
    {
        // Arrange
        UnitErrorArgs errorUnitArgs = new() { ErrorNumber = _errorNo };
        _unitRepository.Setup(r => r.FindByKeyIdAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult<UnitModel>(null));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitErrorService.UpdateErrorAsync(_unitKeyId, errorUnitArgs));

        //Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task UpdateUnitErrorAsync_ReturnsUnitErrorAssignedSuccessfully()
    {
        // Arrange
        UnitModel unitData = new();
        TextModel textData = new();
        UnitErrorArgs errorUnitArgs = new() { ErrorNumber = _errorNo };
        _unitRepository.Setup(r => r.FindByKeyIdAsync(_unitKeyId)).ReturnsAsync(await Task.FromResult(unitData));
        _textService.Setup(r => r.GetErrorAsync(_errorNo)).ReturnsAsync(await Task.FromResult(textData));
        _unitErrorValidator.Setup(r => r.UpdateErrorValidateAsync(_unitKeyId, errorUnitArgs))
            .ReturnsAsync(await Task.FromResult(unitData));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitErrorService.UpdateErrorAsync(_unitKeyId, errorUnitArgs));

        //Assert
        Assert.Null(exception);
        _unitRepository.Verify(r => r.UpdateAsync(unitData), Times.Once);
    }

    #endregion UpdateUnitErrorAsync
}