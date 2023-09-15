using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Patients;

/// <inheritdoc />
public class UnitPatientService : IUnitPatientService
{
    private readonly IUnitPatientValidator _unitPatientValidator;
    private readonly IUnitRepository _unitRepository;
    private readonly IPatientConsumptionRepository _patientConsumptionRepository;
    private readonly IOperationDataRepository _operationDataRepository;

    // <summary>
    /// Initializes a new instance of the <see cref="UnitPatientService" /> class.
    /// </summary>
    /// <param name="unitPatientValidator">Validator provides methods to validate unit patient data.</param>
    /// <param name="unitRepository">Repository provides methods to retrieve/handle units.</param>
    /// <param name="patientConsumptionRepository">Repository provides methods to retrieve/handle patient consumption.</param>
    /// <param name="operationDataRepository">Repository provides methods to retrieve/handle operation data.</param>
    public UnitPatientService(IUnitPatientValidator unitPatientValidator,
        IUnitRepository unitRepository,
        IPatientConsumptionRepository patientConsumptionRepository,
        IOperationDataRepository operationDataRepository)
    {
        _unitPatientValidator = unitPatientValidator;
        _unitRepository = unitRepository;
        _patientConsumptionRepository = patientConsumptionRepository;
        _operationDataRepository = operationDataRepository;
    }

    /// <inheritdoc />
    public async Task UpdatePatientAsync(int unitKeyId, int userKeyId, UnitPatientArgs args)
    {
        await _unitPatientValidator.UpdatePatientValidateAsync(unitKeyId, userKeyId, args);

        UnitModel unit = await _unitPatientValidator.FindByKeyIdValidateAsync(unitKeyId);
        // If PatientKeyId is 0 the assigned patient should be unassigned from the unit.
        if (args.PatientKeyId == 0)
        {
            await _unitRepository.UpdateOperationAsync(unit, null);

            // Removing unit patient consumptions.
            IEnumerable<PatientConsModel> patientConsumptions = await _patientConsumptionRepository.FindAllByUnitKeyIdAsync(unit.KeyId);
            await _patientConsumptionRepository.RemoveRangeAsync(patientConsumptions);

            return;
        }

        int operationKeyId = await _operationDataRepository.GetOperationKeyIdByPatientKeyIdAsync(args.PatientKeyId, args.FactoryKeyId,
            null, userKeyId, args.LocationKeyId, args.PositionLocationKeyId);

        await _unitRepository.UpdateOperationAsync(unit, operationKeyId);

        int? locationKeyId = args.LocationKeyId == 0 ? null : args.LocationKeyId;

        // Updating unit patient consumption.
        PatientConsModel patientConsumption = await _patientConsumptionRepository.FindByUnitKeyIdAsync(unitKeyId);
        if (patientConsumption != null)
        {
            patientConsumption.PatKeyId = args.PatientKeyId;
            patientConsumption.OpDKeyId = operationKeyId;
            patientConsumption.LocaKeyId = locationKeyId;
            patientConsumption.StartUserKeyId = userKeyId;
            patientConsumption.StartTime = DateTime.Now;

            await _patientConsumptionRepository.UpdateAsync(patientConsumption);
            return;
        }

        // Creating a new patient consumption record.
        patientConsumption = new PatientConsModel
        {
            RefUnitUnit = unitKeyId,
            PatKeyId = args.PatientKeyId,
            OpDKeyId = operationKeyId,
            LocaKeyId = locationKeyId,
            StartUserKeyId = userKeyId,
            StartTime = DateTime.Now,
            RefCount = 1 // The value should always be 1 if RefUnitUnit or RefSeriKeyId is set.
        };
        await _patientConsumptionRepository.AddAsync(patientConsumption);
    }
}