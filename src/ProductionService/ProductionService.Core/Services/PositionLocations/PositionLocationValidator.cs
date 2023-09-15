namespace ProductionService.Core.Services.PositionLocations;

/// <inheritdoc cref="IPositionLocationValidator" />
public class PositionLocationValidator : ValidatorBase<PosLocationModel>, IPositionLocationValidator
{
    private readonly IPositionLocationRepository _positionLocationRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PositionLocationValidator" /> class.
    /// </summary>
    /// <param name="positionLocationRepository">Repository provides methods to retrieve/handle position locations.</param>
    public PositionLocationValidator(IPositionLocationRepository positionLocationRepository) : base(positionLocationRepository)
    {
        _positionLocationRepository = positionLocationRepository;
    }
}