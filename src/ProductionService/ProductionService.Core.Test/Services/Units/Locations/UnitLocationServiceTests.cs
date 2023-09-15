using Moq;
using ProductionService.Core.Models.Units;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Units.Locations;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Locations;

public class UnitLocationServiceTests
{
    private const int _unitKeyId = 1001;
    private const int _userKeyId = 1001;
    private const int _locationKeyId = 1001;
    private const int _positionLocationKeyId = 1001;
    private const int _errorNo = 1001;
    private const int _batchNo = 1001;

    // Service to test.
    private readonly UnitLocationService _unitLocationService;

    // Injected services.
    private readonly Mock<IUnitRepository> _unitRepository;

    public UnitLocationServiceTests()
    {
        _unitRepository = new Mock<IUnitRepository>();

        _unitLocationService = new UnitLocationService(_unitRepository.Object);
    }

    #region AddAsync

    [Fact]
    public async Task AddAsync_ErrorIsZeroAndNotAvailableUnit_ThrowsException()
    {
        // Arrange
        _unitRepository.Setup(r => r.GetUnitAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult((UnitModel)null));

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitLocationService.AddAsync(_unitKeyId, _userKeyId, _locationKeyId, _positionLocationKeyId, DateTime.Now, WhatType.Return));

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task AddAsync_AddsUnitLocationRecordSuccessfully()
    {
        // Arrange
        UnitModel unit = new() { KeyId = _unitKeyId };

        _unitRepository.Setup(r => r.GetUnitAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult(unit));

        // Act
        await _unitLocationService.AddAsync(_unitKeyId, _userKeyId, _locationKeyId, _positionLocationKeyId, DateTime.Now, WhatType.Return, _errorNo);

        // Assert
        _unitRepository.Verify(r => r.AddUnitLocationRecordAsync(It.IsAny<UnitLocationData>()), Times.Once);
    }

    #endregion AddAsync


    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_NotAvailableUnit_ThrowsException()
    {
        // Arrange
        _unitRepository.Setup(r => r.GetUnitAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult((UnitModel)null));


        // Act
        Exception exception = await Record.ExceptionAsync(() =>
            _unitLocationService.UpdateAsync(_batchNo, BatchType.PrimarySteri, _unitKeyId, _locationKeyId, DateTime.Now));

        // Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUnitLocationSuccessfully()
    {
        // Arrange
        DateTime locationTime = DateTime.Now;
        UnitModel unit = new() { KeyId = _unitKeyId };

        _unitRepository.Setup(r => r.GetUnitAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult(unit));

        // Act
        await _unitLocationService.UpdateAsync(_batchNo, BatchType.PrimarySteri, _unitKeyId, _locationKeyId, locationTime);

        // Assert
        Assert.Equal(_locationKeyId, unit.LocaKeyId);
        Assert.Equal(locationTime, unit.LocaTime);
        Assert.Equal(_batchNo, unit.Batch);
        _unitRepository.Verify(r => r.UpdateAsync(unit), Times.Once);
    }

    #endregion UpdateAsync
}