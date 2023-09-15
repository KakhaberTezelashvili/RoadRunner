using ProductionService.Shared.Dtos.Patients;

namespace ProductionService.Core.Services.Patients;

/// <summary>
/// Service provides methods to retrieve/handle patients.
/// </summary>
public interface IPatientService
{
    /// <summary>
    /// Retrieves basic details about all existing patients.
    /// </summary>
    /// <returns>List of all existing patients.</returns>
    Task<IList<PatientDetailsBaseDto>> GetPatientsBasicInfoAsync();
}