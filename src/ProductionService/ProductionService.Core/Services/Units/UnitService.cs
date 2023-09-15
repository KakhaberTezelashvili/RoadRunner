using ProductionService.Core.Models.Units;

namespace ProductionService.Core.Services.Units;

/// <inheritdoc cref="IUnitService" />
public class UnitService : IUnitService
{
    private readonly IUnitRepository _unitRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitService" /> class.
    /// </summary>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    public UnitService(IUnitRepository unitRepository)
    {
        _unitRepository = unitRepository;
    }

    /// <inheritdoc />
    public async Task UpdateUnitStatusOrErrorAsync(int unitKeyId, int? status, int? error = null)
    {
        UnitModel unit = await _unitRepository.GetUnitAsync(unitKeyId);

        if (status.HasValue)
            unit.Status = status.Value;

        if (error.HasValue)
            unit.Error = error.Value;

        await _unitRepository.UpdateAsync(unit);
    }

    /// <inheritdoc />
    public Task<IList<UnitModel>> GetByKeyIdsAsync(IEnumerable<int> unitKeyIds) => 
        _unitRepository.GetByKeyIdsAsync(unitKeyIds);
}
