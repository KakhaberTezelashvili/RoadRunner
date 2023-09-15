using Moq;
using Xunit;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Units.Dispatch;
using ProductionService.Shared.Dtos.Units;
using ProductionService.Core.Services.Units.Locations;

namespace ProductionService.Core.Test.Services.Units.Dispatch;

public class UnitDispatchServiceTests
{
    private const int _unitKeyId = 1001;
    private const int _userKeyId = 1001;
    private const int _locationKeyId = 1001;
    private const int _positionLocationKeyId = 1001;
    private const int _errorNo = 1001;
    private const int _customerKeyId = 1001;

    // Service to test.
    private readonly UnitDispatchService _dispatchUnitService;

    // Injected services.
    private readonly Mock<IUnitRepository> _unitRepository;
    private readonly Mock<IUnitDispatchValidator> _dispatchUnitValidator;
    private readonly Mock<IUnitLocationService> _unitLocationService;

    public UnitDispatchServiceTests()
    {
        _unitRepository = new();
        _dispatchUnitValidator = new();
        _unitLocationService = new();

        _dispatchUnitService = new UnitDispatchService(_unitRepository.Object, _dispatchUnitValidator.Object, _unitLocationService.Object);
    }

    #region DispatchAsync

    [Fact]
    public async Task DispatchAsync_UpdatesUnitsSuccessfully()
    {
        // Arrange
        IList<UnitModel> mockUnitsReturnedByValidator = new List<UnitModel>()
            {
                new UnitModel()
                {
                    KeyId = _unitKeyId,
                    CustKeyId = null,
                    Status = (int) UnitStatus.Stock,
                    LocaKeyId = null
                }
            };
        var mockDispatchUnitArgs = new UnitDispatchArgs()
        {
            CustomerKeyId = _customerKeyId,
            PositionLocationKeyId = _positionLocationKeyId,
            LocationKeyId = _locationKeyId
        };
        IList<UnitModel> expectedUnitModels = new List<UnitModel>()
            {
                new UnitModel()
                {
                    KeyId = _unitKeyId,
                    CustKeyId = mockDispatchUnitArgs.CustomerKeyId,
                    LocaKeyId = mockDispatchUnitArgs.LocationKeyId,
                    Status = (int) UnitStatus.Dispatched,
                    Error = _errorNo
                }
            };
        _dispatchUnitValidator.Setup(v => v.DispatchValidateAsync(It.IsAny<UnitDispatchArgs>()))
            .ReturnsAsync(await Task.FromResult(mockUnitsReturnedByValidator));
        _unitRepository.Setup(r => r.GetUnitAsync(It.IsAny<int>()))
            .ReturnsAsync(await Task.FromResult(mockUnitsReturnedByValidator[0]));

        // Act
        await _dispatchUnitService.DispatchAsync(_userKeyId, mockDispatchUnitArgs);

        //Assert
        _unitRepository.Verify(r => r.UpdateAsync(It.IsAny<UnitModel>()), Times.Once);
        _unitLocationService.Verify(r => r.AddAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<DateTime>(), It.IsAny<WhatType>(), It.IsAny<int>()), Times.Once);
    }

    #endregion
}