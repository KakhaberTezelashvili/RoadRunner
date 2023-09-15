using ProductionService.Shared.Dtos.Positions;

namespace ProductionService.Core.Services.Positions;

/// <inheritdoc cref="IPositionService" />
public class PositionService : IPositionService
{
    private readonly IPositionRepository _positionRepository;
    private readonly IPositionValidator _positionValidator;

    /// <summary>
    /// Initilizes a new instance of the <see cref="PositionService" /> class.
    /// </summary>
    /// <param name="positionRepository">Repository provides methods to retrieve/handle positions.</param>
    /// <param name="positionValidator">Validator provides methods to validate positions.</param>
    public PositionService(IPositionRepository positionRepository, IPositionValidator positionValidator)
    {
        _positionRepository = positionRepository;
        _positionValidator = positionValidator;
    }

    /// <inheritdoc />
    public async Task<PositionModel> GetPositionLocationsAsync(int positionKeyId)
    {
        await _positionValidator.FindByKeyIdValidateAsync(positionKeyId);
        return await _positionRepository.GetWithLocationsByKeyIdAsync(positionKeyId);
    }

    /// <inheritdoc />
    public async Task<IList<PositionLocationsDetailsDto>> GetScannerLocationsAsync(int positionKeyId)
    {
        await _positionValidator.FindByKeyIdValidateAsync(positionKeyId);
        return await _positionRepository.GetScannerLocationsByKeyIdAsync(positionKeyId);
    }
}