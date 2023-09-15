using FluentAssertions;
using Moq;
using ProductionService.Core.Repositories.Interfaces;
using ProductionService.Core.Services.Units.Patients;
using ProductionService.Shared.Dtos.Units;
using Xunit;

namespace ProductionService.Core.Test.Services.Units.Patients;

public class UnitPatientServiceTests
{
    private const int _unitKeyId = 1001;
    private const int _userKeyId = 1001;
    private const int _patientKeyId = 1001;
    private const int _factoryKeyId = 1001;
    private const int _locationKeyId = 1001;
    private const int _positionLocationKeyId = 1001;

    // Service to test.
    private readonly UnitPatientService _unitPatientService;

    // Injected services.
    private readonly Mock<IUnitPatientValidator> _unitPatientValidator;
    private readonly Mock<IUnitRepository> _unitRepository;
    private readonly Mock<IPatientConsumptionRepository> _patientConsumptionRepository;
    private readonly Mock<IOperationDataRepository> _operationDataRepository;

    public UnitPatientServiceTests()
    {
        _unitPatientValidator = new();
        _unitRepository = new();
        _patientConsumptionRepository = new();
        _operationDataRepository = new();
        _unitPatientService = new UnitPatientService(_unitPatientValidator.Object, _unitRepository.Object,
            _patientConsumptionRepository.Object, _operationDataRepository.Object);
    }

    #region UpdatePatientAsync

    [Fact]
    public async Task UpdatePatientAsync_ReturnsFailedValidateBeforeSetUnitPatient()
    {
        // Arrange

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitPatientService.UpdatePatientAsync(0, 0, null));

        //Assert
        Assert.NotNull(exception);
    }

    [Fact]
    public async Task UpdatePatientAsync_PatientIdIsZero_UnassignPatientSuccessfully()
    {
        // Arrange
        UnitModel unit = new() { KeyId = _unitKeyId, Status = (int)UnitStatus.Returned };
        UnitPatientArgs args = new()
        {
            FactoryKeyId = _factoryKeyId,
            LocationKeyId = _locationKeyId,
            PositionLocationKeyId = _positionLocationKeyId
        };

        _unitPatientValidator.Setup(r => r.FindByKeyIdValidateAsync(_unitKeyId, null, 1)).ReturnsAsync(await Task.FromResult(unit));
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PatientModel>(_patientKeyId))
            .ReturnsAsync(new PatientModel());

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitPatientService.UpdatePatientAsync(_unitKeyId, _userKeyId, args));

        //Assert
        Assert.Null(exception);
        _unitPatientValidator.Verify(r => r.FindByKeyIdValidateAsync(_unitKeyId, null, 1), Times.Once);
        _unitRepository.Verify(r => r.UpdateOperationAsync(unit, null), Times.Once);
    }

    [Fact]
    public async Task UpdatePatientAsync_PatientIdIsNotZeroAndUnitDoesNotHavePatient_AssignPatientSuccessfully()
    {
        // Arrange
        int operationKeyId = 1;
        UnitModel unit = new() { KeyId = _unitKeyId, Status = (int)UnitStatus.Returned };
        UnitPatientArgs args = new()
        {
            PatientKeyId = 1,
            FactoryKeyId = _factoryKeyId,
            LocationKeyId = _locationKeyId,
            PositionLocationKeyId = _positionLocationKeyId
        };

        _unitPatientValidator.Setup(r => r.FindByKeyIdValidateAsync(_unitKeyId, null, 1)).ReturnsAsync(await Task.FromResult(unit));
        _unitRepository.Setup(r => r.UpdateOperationAsync(It.IsAny<UnitModel>(), It.IsAny<int>())).Returns(Task.CompletedTask);
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PatientModel>(_patientKeyId)).ReturnsAsync(new PatientModel());
        _operationDataRepository
            .Setup(s => s.GetOperationKeyIdByPatientKeyIdAsync(args.PatientKeyId, args.FactoryKeyId, null, _userKeyId, args.LocationKeyId, args.PositionLocationKeyId))
            .ReturnsAsync(operationKeyId);

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitPatientService.UpdatePatientAsync(_unitKeyId, _userKeyId, args));

        //Assert
        Assert.Null(exception);
        _unitPatientValidator.Verify(r => r.FindByKeyIdValidateAsync(_unitKeyId, null, 1), Times.Once);
        _unitRepository.Verify(r => r.UpdateOperationAsync(unit, operationKeyId), Times.Once);
        _operationDataRepository.Verify(r => r.GetOperationKeyIdByPatientKeyIdAsync(args.PatientKeyId, args.FactoryKeyId, null, _userKeyId, args.LocationKeyId,
            args.PositionLocationKeyId), Times.Once);
        _patientConsumptionRepository.Verify(r => r.FindByUnitKeyIdAsync(_unitKeyId), Times.Once);
        _patientConsumptionRepository.Verify(r => r.AddAsync(It.IsAny<PatientConsModel>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePatientAsync_PatientIdIsNotZeroAndUnitHavePatient_UpdatePatientSuccessfully()
    {
        // Arrange
        int operationKeyId = 1;
        UnitModel unit = new() { KeyId = _unitKeyId, Status = (int)UnitStatus.Returned };
        PatientConsModel patient = new();
        UnitPatientArgs args = new()
        {
            PatientKeyId = 1,
            FactoryKeyId = _factoryKeyId,
            LocationKeyId = _locationKeyId,
            PositionLocationKeyId = _positionLocationKeyId
        };

        _unitPatientValidator.Setup(r => r.FindByKeyIdValidateAsync(_unitKeyId, null, 1)).ReturnsAsync(await Task.FromResult(unit));
        _unitRepository.Setup(r => r.UpdateOperationAsync(It.IsAny<UnitModel>(), It.IsAny<int>())).Returns(Task.CompletedTask);
        _unitRepository.Setup(r => r.FindOtherEntityByKeyIdAsync<PatientModel>(_patientKeyId)).ReturnsAsync(new PatientModel());
        _patientConsumptionRepository.Setup(s => s.FindByUnitKeyIdAsync(_unitKeyId)).ReturnsAsync(await Task.FromResult(patient));
        _operationDataRepository
            .Setup(s => s.GetOperationKeyIdByPatientKeyIdAsync(args.PatientKeyId, args.FactoryKeyId, null, _userKeyId, args.LocationKeyId, args.PositionLocationKeyId))
            .ReturnsAsync(operationKeyId);

        // Act
        Exception exception = await Record.ExceptionAsync(() => _unitPatientService.UpdatePatientAsync(_unitKeyId, _userKeyId, args));

        //Assert
        Assert.Null(exception);
        _unitPatientValidator.Verify(r => r.FindByKeyIdValidateAsync(_unitKeyId, null, 1), Times.Once);
        _unitRepository.Verify(r => r.UpdateOperationAsync(unit, operationKeyId), Times.Once);
        _operationDataRepository.Verify(r => r.GetOperationKeyIdByPatientKeyIdAsync(args.PatientKeyId, args.FactoryKeyId, null, _userKeyId,
            args.LocationKeyId, args.PositionLocationKeyId), Times.Once);
        _patientConsumptionRepository.Verify(r => r.FindByUnitKeyIdAsync(_unitKeyId), Times.Once);
        _patientConsumptionRepository.Verify(r => r.UpdateAsync(It.IsAny<PatientConsModel>()), Times.Once);

        patient.Should().BeEquivalentTo(new
        {
            PatKeyId = args.PatientKeyId,
            OpDKeyId = operationKeyId,
            LocaKeyId = _locationKeyId,
            StartUserKeyId = _userKeyId
        }, options => options.ExcludingMissingMembers());
    }

    #endregion UpdatePatientAsync
}