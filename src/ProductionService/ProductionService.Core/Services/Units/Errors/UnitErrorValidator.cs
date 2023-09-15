using ProductionService.Core.Services.Texts;
using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Errors;

/// <inheritdoc cref="IUnitErrorValidator" />
public class UnitErrorValidator : ValidatorBase<UnitModel>, IUnitErrorValidator
{
    private readonly IUnitRepository _unitRepository;
    private readonly ITextService _textService;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitErrorValidator" /> class.
    /// </summary>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    /// <param name="textService">Service provides methods to retrieve/handle error codes, control codes etc.</param>
    public UnitErrorValidator(IUnitRepository unitRepository, ITextService textService) : base(unitRepository)
    {
        _unitRepository = unitRepository;
        _textService = textService;
    }

    /// <inheritdoc />
    public async Task<UnitModel> UpdateErrorValidateAsync(int unitKeyId, UnitErrorArgs args)
    {
        ObjectNullValidate(args);

        UnitModel unit = await FindByKeyIdValidateAsync(unitKeyId);
        TextModel error = await _textService.GetErrorAsync(args.ErrorNumber);
        if (error == null)
            throw ArgumentNotFoundException();

        return unit;
    }
}