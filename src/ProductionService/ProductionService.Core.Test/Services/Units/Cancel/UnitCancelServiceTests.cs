using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Batches;
using ProductionService.Core.Services.Units;
using ProductionService.Core.Services.Units.Cancel;
using ProductionService.Shared.Dtos.Units;
using ProductionService.Shared.Enumerations.Errors;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Cancel;

public class UnitCancelServiceTests
{
    private const int _prevUnitKeyId = 500;
    private const int _unitKeyId = 1001;
    private const int _serialKeyId = 1001;

    // Service to test.
    private readonly UnitCancelService _unitCancelService;

    // Injected services.
    private readonly Mock<IUnitRepository> _unitRepository;
    private readonly Mock<IUnitCancelValidator> _unitCancelValidator;
    private readonly Mock<IUnitValidator> _unitValidator;
    private readonly Mock<IBatchService> _batchService;
    private readonly Mock<ISerialRepository> _serialRepository;

    public UnitCancelServiceTests()
    {
        _unitRepository = new Mock<IUnitRepository>();
        _unitCancelValidator = new();
        _unitValidator = new();
        _batchService = new();
        _serialRepository = new Mock<ISerialRepository>();
        _unitCancelService = new(_unitRepository.Object, _unitCancelValidator.Object, _unitValidator.Object, _batchService.Object, _serialRepository.Object);
    }

    #region CancelAsync

    [Fact]
    public async Task CancelAsync_ReturnsFailedValidateBeforeCancelUnit()
    {
        // Arrange
        UnitCancelArgs args = new();

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitCancelService.CancelAsync(args));

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task CancelAsync_UnitsAreCancelledSuccessfully()
    {
        // Arrange
        UnitCancelArgs args = new()
        {
            UnitKeyIds = new List<int>() { _unitKeyId }
        };

        UnitStatus unitStatus = UnitStatus.Packed;
        UnitModel previousUnit = new() { KeyId = 500, Status = (int)unitStatus, NextUnit = _unitKeyId };
        var unitToBeCancelled = new List<UnitModel>() { new UnitModel() { KeyId = _unitKeyId, Status = (int)unitStatus, PrevUnit = previousUnit.KeyId } };
        
        _unitValidator.Setup(u => u.KeyIdsValidateAsync(new List<int> { (int)unitToBeCancelled.FirstOrDefault().PrevUnit }))
            .ReturnsAsync(new List<UnitModel> { previousUnit });
        _unitCancelValidator.Setup(r => r.CancelValidateAsync(It.IsAny<UnitCancelArgs>()))
            .ReturnsAsync(await Task.FromResult(unitToBeCancelled));

        // Act
        await _unitCancelService.CancelAsync(args);

        // Assert
        Assert.All(unitToBeCancelled, item => Assert.Equal((int)UnitStatus.ErrorReg, item.Status));
        Assert.All(unitToBeCancelled, item => Assert.Equal((int)FixErrorCodes.ErrorReg, item.Error));
        Assert.All(unitToBeCancelled, item => Assert.Null(item.StokKeyId));
        Assert.Null(previousUnit.NextUnit);
        _unitRepository.Verify(r => r.UpdateAsync(unitToBeCancelled.FirstOrDefault()), Times.Once);
        _unitRepository.Verify(r => r.UpdateAsync(previousUnit), Times.Once);
        _batchService.Verify(b => b.RemoveBatchesByUnitKeyIdAndUnitStatusAsync(_unitKeyId, unitStatus), Times.Once);
    }

    [Theory]
    [InlineData(10, 9, _unitKeyId, _prevUnitKeyId, _prevUnitKeyId)]
    [InlineData(1, null, _unitKeyId, 0, null)]
    [InlineData(0, null, _unitKeyId, null, null)]
    public async Task CancelUnitAsync_SerialDataIsUpdated(int? usageCounter, int? expectedUsage, int unitId, int? prevUnitId, int? expecteUnitdId)
    {
        // Arrange
        UnitStatus unitStatus = UnitStatus.Packed;
        UnitModel previousUnit = null;
        if (prevUnitId.HasValue)
        {
            previousUnit = new() { KeyId = prevUnitId.Value, Status = (int)unitStatus, NextUnit = unitId };
            _unitValidator.Setup(u => u.KeyIdsValidateAsync(new List<int> { prevUnitId.Value })).ReturnsAsync(new List<UnitModel> { previousUnit });
        }

        var unitToBeCancelled = new List<UnitModel>() {
            new UnitModel() {
                KeyId = unitId,
                Status = (int)unitStatus,
                PrevUnit = previousUnit?.KeyId,
                UsageCounter = usageCounter,
                SeriKeyId = _serialKeyId } };

        var serialModel = new SerialModel() { KeyId = _serialKeyId, UsageCount = 1, UnitUnit = (int)unitToBeCancelled.FirstOrDefault().KeyId };
        _unitValidator.Setup(u => u.FindOtherEntityByKeyIdValidateAsync<SerialModel>((int)unitToBeCancelled.FirstOrDefault().SeriKeyId, null, 1)).ReturnsAsync(serialModel);

        UnitCancelArgs args = new()
        {
            UnitKeyIds = new List<int>() { _unitKeyId }
        };

        _unitCancelValidator.Setup(r => r.CancelValidateAsync(It.IsAny<UnitCancelArgs>()))
            .ReturnsAsync(await Task.FromResult(unitToBeCancelled));

        // Act
        await _unitCancelService.CancelAsync(args);

        // Assert
        Assert.Equal(expecteUnitdId, serialModel.UnitUnit);
        Assert.Equal(expectedUsage, serialModel.UsageCount);
        _serialRepository.Verify(r => r.UpdateAsync(serialModel), Times.Once);
    }

    #endregion CancelAsync
}