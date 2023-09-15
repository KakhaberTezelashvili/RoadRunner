using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Patients;

/// <inheritdoc cref="IUnitPatientValidator" />
public class UnitPatientValidator : ValidatorBase<UnitModel>, IUnitPatientValidator
{
    private readonly IUnitRepository _unitRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitPatientValidator" /> class.
    /// </summary>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    public UnitPatientValidator(IUnitRepository unitRepository) : base(unitRepository)
    {
        _unitRepository = unitRepository;
    }

    /// <inheritdoc />
    public async Task<UnitModel> UpdatePatientValidateAsync(int unitKeyId, int userKeyId, UnitPatientArgs args)
    {
        ObjectNullValidate(args);

        bool isValid = userKeyId > 0 &&
                      unitKeyId > 0 &&
                      (args.PatientKeyId == 0 ||
                       args.PatientKeyId > 0 && args.LocationKeyId > 0 && args.PositionLocationKeyId > 0 && args.FactoryKeyId > 0);

        if (!isValid)
            throw ArgumentNotValidException();

        UnitModel unit = await FindByKeyIdValidateAsync(unitKeyId);
        if (unit.Status != (int)UnitStatus.Returned)
            throw InputArgumentException(InputArgumentPatientErrorCodes.PatientStatusNotValid);

        if (args.PatientKeyId != 0)
            await FindOtherEntityByKeyIdValidateAsync<PatientModel>(args.PatientKeyId);

        return unit;
    }
}