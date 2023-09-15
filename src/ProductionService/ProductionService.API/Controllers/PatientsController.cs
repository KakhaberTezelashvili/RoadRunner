using ProductionService.Core.Services.Patients;
using ProductionService.Shared.Dtos.Patients;

namespace ProductionService.API.Controllers;

/// <summary>
/// EF controller provides methods to retrieve/handle patients.
/// </summary>
public class PatientsController : ApiControllerBase
{
    private readonly IPatientService _patientService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PatientsController" /> class.
    /// </summary>
    /// <param name="patientService">Service provides methods to retrieve/handle patients.</param>
    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    // GET: api/v1/patients/basic-info
    /// <summary>
    /// Retrieves basic information about all existing patients.
    /// </summary>
    /// <returns>
    /// Action result indicating the result of the operation; if the operation was successful,
    /// the list of patients is returned as part of the response.
    /// </returns>
    [HttpGet("basic-info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<IList<PatientDetailsBaseDto>>> GetPatientsBasicInfoAsync() =>
        (await _patientService.GetPatientsBasicInfoAsync()).ToList();
}