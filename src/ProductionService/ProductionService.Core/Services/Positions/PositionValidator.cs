namespace ProductionService.Core.Services.Positions;

/// <inheritdoc cref="IPositionValidator" />
public class PositionValidator : ValidatorBase<PositionModel>, IPositionValidator
{
    private readonly IPositionRepository _positionRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PositionValidator" /> class.
    /// </summary>
    /// <param name="positionRepository">Repository provides methods to retrieve/handle positions.</param>
    public PositionValidator(IPositionRepository positionRepository) : base(positionRepository)
    {
        _positionRepository = positionRepository;
    }
}