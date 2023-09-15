namespace ProductionService.Core.Services.Patients;

/// <inheritdoc cref="IPatientValidator" />
public class PatientValidator : ValidatorBase<PatientModel>, IPatientValidator
{
    private readonly IPatientRepository _patientRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatientValidator" /> class.
    /// </summary>
    /// <param name="machineRepository">Repository provides methods to retrieve/handle patients.</param>
    public PatientValidator(IPatientRepository patientRepository) : base(patientRepository)
    {
        _patientRepository = patientRepository;
    }
}