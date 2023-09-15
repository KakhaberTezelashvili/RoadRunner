using ProductionService.Core.Services.Units.Locations;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Dispatch;

/// <inheritdoc cref="IUnitDispatchService" />
public class UnitDispatchService : IUnitDispatchService
{
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitDispatchValidator _dispatchUnitValidator;
    private readonly IUnitLocationService _unitLocationService;

    /// <summary>
    /// Creates a new instance of the <see cref="UnitDispatchService"/> class.
    /// </summary>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    /// <param name="dispatchUnitValidator">Validator provides methods to validate unit dispatch data.</param>
    /// <param name="unitLocationService">Service provides methods to retrieve/handle unit locations.</param>
    public UnitDispatchService(IUnitRepository unitRepository, IUnitDispatchValidator dispatchUnitValidator, IUnitLocationService unitLocationService)
    {
        _unitRepository = unitRepository;
        _dispatchUnitValidator = dispatchUnitValidator;
        _unitLocationService = unitLocationService;
    }

    /// <inheritdoc />
    /// For the details, look at delphi method "TOutHandler.DeliverUnit"
    public async Task DispatchAsync(int userKeyId, UnitDispatchArgs args)
    {
        IList<UnitModel> units = await _dispatchUnitValidator.DispatchValidateAsync(args);
        foreach (UnitModel unit in units)
        {
            unit.CustKeyId = args.CustomerKeyId;
            unit.Status = (int)UnitStatus.Dispatched;
            unit.LocaKeyId = args.LocationKeyId;
            await _unitRepository.UpdateAsync(unit);
            await _unitLocationService.AddAsync(
                unit.KeyId, userKeyId, args.LocationKeyId, args.PositionLocationKeyId, DateTime.Now, WhatType.Out);
        }
    }
}