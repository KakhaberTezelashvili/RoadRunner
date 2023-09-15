using Moq;
using ProductionService.Core.Models.FastTracking;
using ProductionService.Core.Models.Units;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Core.Services.FastTracking;
using ProductionService.Core.Services.Units.Locations;
using ProductionService.Core.Services.Units.Pack;
using ProductionService.Shared.Dtos.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Pack;

public class UnitPackServiceTests
{
    private const int _unitKeyId = 1001;
    private const int _newUnitKeyId = 1002;
    private const int _productSerialKeyId = 1001;
    private const int _userKeyId = 1001;
    private readonly string _fastTrackName = "Any FT name";
    private readonly FastTrackDisplayInfo _fastTrackDisplayInfo;

    // Service to test.
    private readonly UnitPackService _unitPackService;

    // Injected services.
    private readonly Mock<IUnitPackRepository> _unitPackRepository;
    private readonly Mock<IUnitRepository> _unitRepository;
   
    private readonly Mock<IFastTrackService> _ftService;
    private readonly Mock<IUnitPackValidator> _unitPackValidator;
    private readonly Mock<IUnitLocationService> _unitLocationService;

    public UnitPackServiceTests()
    {
        _fastTrackDisplayInfo = new FastTrackDisplayInfo();
        _fastTrackDisplayInfo.AddFastTrackByCode(1, "Any FT code", _fastTrackName, 1, FastTrackDisplayMode.UserSetting);
        _unitPackRepository = new Mock<IUnitPackRepository>();
        _unitRepository = new Mock<IUnitRepository>();
        _ftService = new Mock<IFastTrackService>();
        _unitPackValidator = new Mock<IUnitPackValidator>();
        _unitLocationService = new Mock<IUnitLocationService>();
        _unitPackService = new UnitPackService(
            _unitPackRepository.Object, _unitRepository.Object, _ftService.Object, _unitPackValidator.Object, _unitLocationService.Object);
    }

    #region GetPackDetailsAsync

    [Fact]
    public async Task GetPackDetailsAsync_ReturnsNothing()
    {
        // Arrange
        _unitPackRepository.Setup(r => r.GetPackDetailsAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult<UnitPackDetailsDto>(null));

        // Act
        UnitPackDetailsDto unitDetails = await _unitPackService.GetPackDetailsAsync(_unitKeyId);

        // Assert
        Assert.Null(unitDetails);
    }

    [Fact]
    public async Task GetPackDetailsAsync_ReturnsUnitDetailsForPack()
    {
        // Arrange
        _unitPackRepository.Setup(r => r.GetPackDetailsAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult(new UnitPackDetailsDto()));

        _ftService.Setup(r => r.GetUnitFastTrackDisplayInfoAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult(_fastTrackDisplayInfo));

        // Act
        UnitPackDetailsDto unitDetails = await _unitPackService.GetPackDetailsAsync(_unitKeyId);

        // Assert
        Assert.NotNull(unitDetails);
        Assert.Equal(_fastTrackName, unitDetails.FastTrackName);
    }

    #endregion GetPackDetailsAsync

    #region PackAsync

    [Fact]
    public async Task PackAsync_ReturnsFailedValidateBeforePack()
    {
        // Arrange
        _unitPackRepository.Setup(r => r.GetDataToPackAsync(null))
            .ReturnsAsync(await Task.FromResult<UnitDataToPack>(null));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitPackService.PackAsync(_userKeyId, null));

        // Assert
        Assert.NotNull(exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(_productSerialKeyId)]
    public async Task PackAsync_ReturnsNewPackedUnitAndExecutedSetNextUnitForUnitAndProductSerialOnDemand(int productSerialKeyId)
    {
        // Arrange
        const int usageCount = 1;
        UnitDataToPack packUnit = new() { UnitKeyId = _unitKeyId, ProductSerialKeyId = productSerialKeyId };
        UnitPackArgs packUnitArgs = new();

        _unitPackRepository.Setup(r => r.GetDataToPackAsync(packUnitArgs))
            .ReturnsAsync(await Task.FromResult(packUnit));
        _unitRepository.Setup(r => r.GetUnitAsync(It.IsAny<int>()))
            .ReturnsAsync(await Task.FromResult(new UnitModel()));
        _unitRepository.Setup(r => r.AddDataAsync(It.IsAny<UnitModel>()))
            .ReturnsAsync(await Task.FromResult(_newUnitKeyId));
        _unitPackValidator.Setup(r => r.PackValidateAsync(packUnitArgs))
            .ReturnsAsync(await Task.FromResult(packUnit));

        // Act
        IEnumerable<int> unitKeyIds = await _unitPackService.PackAsync(_userKeyId, packUnitArgs);

        // Assert
        Assert.Equal(_newUnitKeyId, unitKeyIds.First());
        _unitRepository.Verify(r => r.SetUnitNextUnitAsync(_unitKeyId, _newUnitKeyId), Times.Once);
        _unitRepository.Verify(r => r.UpdateUnitDataForProductSerialAsync(productSerialKeyId, usageCount, _newUnitKeyId),
            productSerialKeyId == 0 ? Times.Never : Times.Once);
        _unitLocationService.Verify(r => r.AddAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<DateTime>(), It.IsAny<WhatType>(), It.IsAny<int>()), Times.Once);
    }

    #endregion PackAsync
}