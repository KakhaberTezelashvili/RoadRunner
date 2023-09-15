using Moq;
using ProductionService.Core.Models.FastTracking;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Core.Services.FastTracking;
using ProductionService.Core.Services.Units;
using ProductionService.Core.Services.Units.Locations;
using ProductionService.Core.Services.Units.Patients;
using ProductionService.Core.Services.Units.Return;
using ProductionService.Shared.Dtos.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Return;

public class UnitReturnServiceTests
{
    private const int _unitKeyId = 1001;
    private const int _productSerialKeyId = 1001;
    private const int _userKeyId = 1001;
    private const int _patientKeyId = 1001;
    private const int _factoryKeyId = 1001;
    private const int _locationKeyId = 1001;
    private const int _positionLocationKeyId = 1001;
    private readonly string _fastTrackName = "Any FT name";
    private readonly FastTrackDisplayInfo _fastTrackDisplayInfo;

    // Service to test.
    private readonly UnitReturnService _unitReturnService;

    // Injected services.
    private readonly Mock<IUnitReturnRepository> _unitReturnRepository;
    private readonly Mock<IUnitRepository> _unitRepository;
    private readonly Mock<IFastTrackService> _ftService;
    private readonly Mock<IUnitLocationService> _unitLocationService;
    private readonly Mock<IUnitReturnValidator> _unitReturnValidator;
    private readonly Mock<IUnitPatientService> _unitPatientService;
    private readonly Mock<IUnitService> _unitService;

    public UnitReturnServiceTests()
    {
        _fastTrackDisplayInfo = new FastTrackDisplayInfo();
        _fastTrackDisplayInfo.AddFastTrackByCode(1, "Any FT code", _fastTrackName, 1, FastTrackDisplayMode.UserSetting);

        _unitReturnRepository = new();
        _unitRepository = new();
        _ftService = new();
        _unitLocationService = new();
        _unitReturnValidator = new();
        _unitPatientService = new();
        _unitService = new();
        _unitReturnService = new(_unitReturnRepository.Object, _unitRepository.Object, _ftService.Object,
            _unitLocationService.Object, _unitReturnValidator.Object, _unitPatientService.Object, _unitService.Object);
    }

    #region GetReturnDetailsAsync

    [Fact]
    public async Task GetReturnDetailsAsync_ReturnsNothing()
    {
        // Arrange
        _unitReturnRepository.Setup(r => r.GetReturnDetailsAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult<UnitReturnDetailsDto>(null));

        // Act
        UnitReturnDetailsDto unitDetails = await _unitReturnService.GetReturnDetailsAsync(_unitKeyId);

        //Assert
        Assert.Null(unitDetails);
    }

    [Fact]
    public async Task GetReturnDetailsAsync_ReturnsUnitReturnDetails()
    {
        // Arrange
        _unitReturnRepository.Setup(r => r.GetReturnDetailsAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult(new UnitReturnDetailsDto()));

        _ftService.Setup(r => r.GetUnitFastTrackDisplayInfoAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult(_fastTrackDisplayInfo));

        // Act
        UnitReturnDetailsDto unitDetails = await _unitReturnService.GetReturnDetailsAsync(_unitKeyId);

        //Assert
        Assert.NotNull(unitDetails);
        Assert.Equal(_fastTrackName, unitDetails.FastTrackName);
    }

    #endregion GetReturnDetailsAsync

    #region ReturnAsync

    [Theory]
    [InlineData(0)]
    [InlineData(_productSerialKeyId)]
    public async Task ReturnAsync_UnitIsReturned_StatusShouldBeReturned(int productSerialKeyId)
    {
        // Arrange
        const int usageCount = 1;
        UnitModel unit = new() { KeyId = _unitKeyId, Status = (int)UnitStatus.Packed };
        UnitReturnArgs args = new()
        {
            UnitKeyId = _unitKeyId,
            FactoryKeyId = _factoryKeyId,
            LocationKeyId = _locationKeyId,
            PositionLocationKeyId = _positionLocationKeyId,
            ProductSerialKeyId = productSerialKeyId
        };

        _unitRepository.Setup(r => r.GetUnitAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult(unit));
        _unitReturnValidator.Setup(r => r.ReturnValidateAsync(It.IsAny<int>(), It.IsAny<UnitReturnArgs>()))
            .ReturnsAsync(await Task.FromResult(unit));

        // Act
        int unitKeyId = await _unitReturnService.ReturnAsync(_userKeyId, args);

        //Assert
        Assert.Equal(_unitKeyId, unitKeyId);
        _unitService.Verify(r => r.UpdateUnitStatusOrErrorAsync(_unitKeyId, (int)UnitStatus.Returned, unit.Error), Times.Once);
        _unitRepository.Verify(r => r.UpdateUnitDataForProductSerialAsync(productSerialKeyId, usageCount, _unitKeyId),
            productSerialKeyId == 0 ? Times.Never : Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(_productSerialKeyId)]
    public async Task ReturnAsync_ArgsHasErrorNumber_UnitErrorMustBeSet(int productSerialKeyId)
    {
        // Arrange
        const int usageCount = 1;
        UnitModel unit = new() { KeyId = _unitKeyId, Error = 0, Status = (int)UnitStatus.Packed };
        UnitReturnArgs args = new()
        {
            UnitKeyId = _unitKeyId,
            Error = 1,
            FactoryKeyId = _factoryKeyId,
            LocationKeyId = _locationKeyId,
            PositionLocationKeyId = _positionLocationKeyId,
            ProductSerialKeyId = productSerialKeyId
        };

        _unitRepository.Setup(r => r.GetUnitAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult(unit));
        _unitReturnValidator.Setup(r => r.ReturnValidateAsync(It.IsAny<int>(), It.IsAny<UnitReturnArgs>()))
            .ReturnsAsync(await Task.FromResult(unit));

        // Act
        int unitKeyId = await _unitReturnService.ReturnAsync(_userKeyId, args);

        //Assert
        Assert.Equal(_unitKeyId, unitKeyId);
        _unitService.Verify(r => r.UpdateUnitStatusOrErrorAsync(unit.KeyId, (int)UnitStatus.Returned, args.Error), Times.Once);
        _unitRepository.Verify(r => r.UpdateUnitDataForProductSerialAsync(productSerialKeyId, usageCount, _unitKeyId),
            productSerialKeyId == 0 ? Times.Never : Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(_productSerialKeyId)]
    public async Task ReturnAsync_PatientKeyIdIsNotZero_PatientShouldBeAssignedToUnit(int productSerialKeyId)
    {
        // Arrange
        const int usageCount = 1;
        UnitModel unit = new() { KeyId = _unitKeyId, Status = (int)UnitStatus.Packed };
        UnitReturnArgs args = new()
        {
            UnitKeyId = _unitKeyId,
            PatientKeyId = _patientKeyId,
            FactoryKeyId = _factoryKeyId,
            LocationKeyId = _locationKeyId,
            PositionLocationKeyId = _positionLocationKeyId,
            ProductSerialKeyId = productSerialKeyId
        };

        _unitRepository.Setup(r => r.GetUnitAsync(_unitKeyId))
            .ReturnsAsync(await Task.FromResult(unit));
        _unitReturnValidator.Setup(r => r.ReturnValidateAsync(It.IsAny<int>(), It.IsAny<UnitReturnArgs>()))
            .ReturnsAsync(await Task.FromResult(unit));

        // Act
        int unitKeyId = await _unitReturnService.ReturnAsync(_userKeyId, args);

        //Assert
        Assert.Equal(_unitKeyId, unitKeyId);
        _unitLocationService.Verify(r => r.AddAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<DateTime>(), It.IsAny<WhatType>(), It.IsAny<int>()), Times.Once);
        _unitRepository.Verify(r => r.UpdateUnitDataForProductSerialAsync(productSerialKeyId, usageCount, _unitKeyId),
            productSerialKeyId == 0 ? Times.Never : Times.Once);
        _unitPatientService.Verify(r => r.UpdatePatientAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<UnitPatientArgs>()), Times.Once);
    }

    #endregion ReturnAsync
}