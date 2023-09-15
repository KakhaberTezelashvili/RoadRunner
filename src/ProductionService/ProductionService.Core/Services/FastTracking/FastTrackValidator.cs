namespace ProductionService.Core.Services.FastTracking;

/// <inheritdoc cref="IFastTrackValidator" />
public class FastTrackValidator : ValidatorBase<UnitFastTrackModel>, IFastTrackValidator
{
    private readonly IFastTrackRepository _fastTrackRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="FastTrackValidator" /> class.
    /// </summary>
    /// <param name="fastTrackRepository">Repository provides methods to retrieve/handle fast track data.</param>
    public FastTrackValidator(IFastTrackRepository fastTrackRepository) : base(fastTrackRepository)
    {
        _fastTrackRepository = fastTrackRepository;
    }

    /// <inheritdoc />
    public void UnitFastTrackDisplayInfoValidate(int unitKeyId)
    {
        if (unitKeyId <= 0)
            throw ArgumentNotValidException();
    }
}