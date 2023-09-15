using ProductionService.Shared.Dtos.Patients;

namespace ProductionService.Core.Services.Patients;

/// <inheritdoc cref="IPatientService" />
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientValidator _patientValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatientService" /> class.
    /// </summary>
    /// <param name="patientRepository">Repository provides methods to retrieve/handle patients.</param>
    /// <param name="patientValidator">Validator provides methods to validate patients.</param>
    public PatientService(IPatientRepository patientRepository, IPatientValidator patientValidator)
    {
        _patientRepository = patientRepository;
        _patientValidator = patientValidator;
    }

    /// <inheritdoc />
    public async Task<IList<PatientDetailsBaseDto>> GetPatientsBasicInfoAsync() => await _patientRepository.GetAllBasicInfosAsync();
}