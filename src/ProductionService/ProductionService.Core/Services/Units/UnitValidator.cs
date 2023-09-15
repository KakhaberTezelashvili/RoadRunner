namespace ProductionService.Core.Services.Units;

/// <inheritdoc cref="IUnitValidator" />
public class UnitValidator : ValidatorBase<UnitModel>, IUnitValidator
{
    private readonly IUnitRepository _unitRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitValidator" /> class.
    /// </summary>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    public UnitValidator(IUnitRepository unitRepository) : base(unitRepository)
    {
        _unitRepository = unitRepository;
    }

    /// <inheritdoc />
    public async Task<IList<UnitModel>> KeyIdsValidateAsync(IList<int> unitKeyIds)
    {
        IList<UnitModel> units = new List<UnitModel>();
        foreach (int unitKeyId in unitKeyIds)
            units.Add(await FindByKeyIdValidateAsync(unitKeyId));

        return units;
    }
}