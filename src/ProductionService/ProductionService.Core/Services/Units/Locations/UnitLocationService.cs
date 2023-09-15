using ProductionService.Core.Models.Units;

namespace ProductionService.Core.Services.Units.Locations;

/// <inheritdoc />
public class UnitLocationService : IUnitLocationService
{
    private readonly IUnitRepository _unitRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitLocationService" /> class.
    /// </summary>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    public UnitLocationService(IUnitRepository unitRepository)
    {
        _unitRepository = unitRepository;
    }

    /// <inheritdoc />
    public async Task AddAsync(int unitKeyId, int userKeyId, int locationKeyId, int positionLocationKeyId, DateTime locationTime, WhatType what, int error = 0)
    {
        UnitModel unit = await _unitRepository.GetUnitAsync(unitKeyId);

        var unitLocationData = new UnitLocationData
        {
            What = what,
            UnitKeyId = unitKeyId,
            Error = error > 0 ? error : unit.Error,
            LocationKeyId = locationKeyId,
            PositionLocationKeyId = positionLocationKeyId,
            UserKeyId = userKeyId,
            Time = locationTime
        };
        await _unitRepository.AddUnitLocationRecordAsync(unitLocationData);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(int batchKeyId, BatchType batchType, int unitKeyId, int locationKeyId, DateTime locationTime)
    {
        UnitModel unit = await _unitRepository.GetUnitAsync(unitKeyId);
        unit.LocaKeyId = locationKeyId;
        unit.LocaTime = locationTime;

        switch (batchType)
        {
            case BatchType.PrimarySteri:
                unit.Batch = batchKeyId;
                break;

            // TODO:
            case BatchType.PostWash:
                break;
        }

        await _unitRepository.UpdateAsync(unit);
    }
}