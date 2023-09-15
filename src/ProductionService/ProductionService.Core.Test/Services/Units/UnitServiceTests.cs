using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Units;

public class UnitServiceTests
{
    private const int _unitKeyId = 1001;
    private const int _errorNo = 1001;

    // Service to test.
    private readonly UnitService _unitService;

    // Injected services.
    private readonly Mock<IUnitRepository> _unitRepository;

    public UnitServiceTests()
    {
        _unitRepository = new();
        _unitService = new(_unitRepository.Object);
    }

    #region UpdateUnitStatusAsync

    [Fact]
    public async Task UpdateUnitStatusAsync_NotAvailableUnit_ThrowsException()
    {
        // Arrange
        _unitRepository.Setup(r => r.GetUnitAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult((UnitModel)null));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitService.UpdateUnitStatusOrErrorAsync(_unitKeyId, (int)UnitStatus.Returned, _errorNo));

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task UpdateUnitStatusAsync_UpdatesUnitStatusSuccessfully()
    {
        // Arrange
        int status = (int)UnitStatus.Returned;
        UnitModel unit = new() { KeyId = _unitKeyId };

        _unitRepository.Setup(r => r.GetUnitAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult(unit));

        // Act
        await _unitService.UpdateUnitStatusOrErrorAsync(_unitKeyId, status, _errorNo);

        // Assert
        Assert.Equal(status, unit.Status);
        Assert.Equal(_errorNo, unit.Error);
        _unitRepository.Verify(r => r.UpdateAsync(unit), Times.Once);
    }

    #endregion UpdateUnitStatusAsync

    #region GetByKeyIdsAsync

    [Fact]
    public async Task GetByKeyIdsAsync_ReturnsNothing()
    {
        // Arrange
        IList<int> mockUnitKeyIds = new List<int>(_unitKeyId);

        _unitRepository.Setup(r => r.GetByKeyIdsAsync(mockUnitKeyIds)).ReturnsAsync(
            await Task.FromResult<IList<UnitModel>>(null));

        // Act
        IList<UnitModel> unitModels = await _unitService.GetByKeyIdsAsync(mockUnitKeyIds);

        // Assert
        Assert.Null(unitModels);
    }

    [Fact]
    public async Task GetByKeyIdsAsync_ReturnsUnitModels()
    {
        // Arrange
        IList<int> mockUnitKeyIds = new List<int>(_unitKeyId);
        IList<UnitModel> mockUnitModels = new List<UnitModel>
        {
            new UnitModel { KeyId = _unitKeyId },
            new UnitModel { KeyId = _unitKeyId }
        };

        _unitRepository.Setup(r => r.GetByKeyIdsAsync(mockUnitKeyIds)).ReturnsAsync(
            await Task.FromResult(mockUnitModels));

        // Act
        IList<UnitModel> unitModels = await _unitService.GetByKeyIdsAsync(mockUnitKeyIds);

        // Assert
        Assert.NotNull(unitModels);
        Assert.Equal(unitModels.Count, unitModels.Count);
        Assert.Equal(unitModels[0].KeyId, mockUnitModels[0].KeyId);
        Assert.Equal(unitModels[1].KeyId, mockUnitModels[1].KeyId);
    }

    #endregion
}