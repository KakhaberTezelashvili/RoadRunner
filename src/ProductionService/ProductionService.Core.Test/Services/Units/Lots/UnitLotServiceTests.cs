using Moq;
using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Core.Services.Units.Lots;
using ProductionService.Core.Services.Units.Lots;
using ProductionService.Shared.Dtos.Lots;
using ProductionService.Shared.Dtos.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Lots;

public class UnitLotServiceTests
{
    private const int _unitKeyId = 1001;
    private const int _userKeyId = 1001;
    private const int _locationKeyId = 1;
    private readonly List<UnitLotInformationArgs> _lotInfoList;

    // Service to test.
    private readonly UnitLotService _unitLotService;

    // Injected services.
    private readonly Mock<IUnitLotRepository> _unitLotRepository;
    private readonly Mock<IUnitLotValidator> _unitLotValidator;

    public UnitLotServiceTests()
    {
        _unitLotRepository = new();
        _unitLotValidator = new();
        _lotInfoList = new()
        {
            new UnitLotInformationArgs { KeyId = 1, Position = 1 }
        };
        _unitLotService = new(_unitLotValidator.Object, _unitLotRepository.Object);
    }

    #region UpdateLotsAsync

    [Fact]
    public async Task UpdateLotsAsync_ReturnsFailedValidateBeforeUpdateUnitLOTInformation()
    {
        // Arrange
        var updateUnitLotInformationArgs = new UnitLotsArgs();
        _unitLotValidator.Setup(v => v.UpdateLotsValidateAsync(updateUnitLotInformationArgs)).Throws(new Exception());
        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitLotService.UpdateLotsAsync(updateUnitLotInformationArgs));

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task UpdateLotsAsync_ReturnsUpdateUnitLOTInformation()
    {
        // Arrange
        var updateUnitLotInformationArgs = new UnitLotsArgs
        {
            LocationKeyId = _locationKeyId,
            LotInformationList = _lotInfoList,
            UnitKeyId = _unitKeyId,
            UserKeyId = _userKeyId
        };
        _unitLotRepository.Setup(r => r.RemoveRangeAsync(It.IsAny<IEnumerable<UnitLotInfoModel>>())).Returns(Task.CompletedTask);
        _unitLotRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<UnitLotInfoModel>>())).Returns(Task.CompletedTask);

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitLotService.UpdateLotsAsync(updateUnitLotInformationArgs));

        // Assert
        Assert.Null(exception);
    }

    #endregion UpdateUnitLOTInformationAsync
}