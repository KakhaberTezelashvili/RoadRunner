using ProductionService.Core.Repositories.Interfaces.Units;
using ProductionService.Core.Services.FastTracking;
using ProductionService.Core.Services.Units.Locations;
using ProductionService.Core.Services.Units.Patients;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Return;

/// <inheritdoc cref="IUnitReturnService" />
public class UnitReturnService : IUnitReturnService
{
    private readonly IUnitReturnRepository _unitReturnRepository;
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitReturnValidator _unitReturnValidator;
    private readonly IFastTrackService _ftService;
    private readonly IUnitLocationService _unitLocationService;
    private readonly IUnitPatientService _unitPatientService;
    private readonly IUnitService _unitService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitReturnService" /> class.
    /// </summary>
    /// <param name="unitReturnRepository">Repository provides methods to retrieve/handle unit return data.</param>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    /// <param name="fastTrackService">Service provides methods to retrieve/handle fast track.</param>
    /// <param name="unitLocationService">Service provides methods to retrieve/handle unit locations.</param>
    /// <param name="unitReturnValidator">Validator provides methods to validate unit return data.</param>
    /// <param name="unitPatientService">Service provides methods to retrieve/handle unit patient data.</param>
    /// <param name="unitService">Service provides methods to retrieve/handle units</param>
    public UnitReturnService(
        IUnitReturnRepository unitReturnRepository,
        IUnitRepository unitRepository,
        IFastTrackService fastTrackService,
        IUnitLocationService unitLocationService,
        IUnitReturnValidator unitReturnValidator,
        IUnitPatientService unitPatientService,
        IUnitService unitService)
    {
        _unitReturnRepository = unitReturnRepository;
        _unitRepository = unitRepository;
        _ftService = fastTrackService;
        _unitLocationService = unitLocationService;
        _unitReturnValidator = unitReturnValidator;
        _unitPatientService = unitPatientService;
        _unitService = unitService;
}

    /// <inheritdoc />
    public async Task<UnitReturnDetailsDto> GetReturnDetailsAsync(int unitKeyId)
    {
        UnitReturnDetailsDto info = await _unitReturnRepository.GetReturnDetailsAsync(unitKeyId);
        if (info != null)
            info.FastTrackName = (await _ftService.GetUnitFastTrackDisplayInfoAsync(unitKeyId))?.Name;
        return info;
    }

    /// <inheritdoc />
    public async Task<int> ReturnAsync(int userKeyId, UnitReturnArgs args)
    {
        UnitModel unit = await _unitReturnValidator.ReturnValidateAsync(userKeyId, args);

        await _unitService.UpdateUnitStatusOrErrorAsync(unit.KeyId, (int)UnitStatus.Returned, args.Error > 0 ? args.Error : unit.Error);
        await _unitLocationService.AddAsync(
            unit.KeyId, userKeyId, args.LocationKeyId, args.PositionLocationKeyId, DateTime.Now, WhatType.Return, args.Error);

        if (args.ProductSerialKeyId > 0)
            await _unitRepository.UpdateUnitDataForProductSerialAsync(args.ProductSerialKeyId, 1/* todo UsageCount */, unit.KeyId);

        if (args.PatientKeyId == 0)
            return unit.KeyId;

        // Unit data might come from serial and to ensure Patient Args unit key will be set.
        args.UnitKeyId = unit.KeyId;
        UnitPatientArgs patientArgs = AsPatientUnitArgs(args);

        await _unitPatientService.UpdatePatientAsync(unit.KeyId, userKeyId, patientArgs);

        return unit.KeyId;
    }

    /// <summary>
    /// Converts <see cref="UnitReturnArgs" /> class to the <see cref="UnitPatientArgs" /> class.
    /// </summary>
    /// <param name="args">Unit return arguments.</param>
    /// <returns>Unit patient arguments.</returns>
    private UnitPatientArgs AsPatientUnitArgs(UnitReturnArgs args)
    {
        return new UnitPatientArgs
        {
            FactoryKeyId = args.FactoryKeyId,
            LocationKeyId = args.LocationKeyId,
            PatientKeyId = args.PatientKeyId,
            PositionLocationKeyId = args.PositionLocationKeyId
        };
    }
}