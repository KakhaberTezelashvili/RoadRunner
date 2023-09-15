using ProductionService.Shared.Dtos.Patients;

namespace ProductionService.Client.Services.Patients;

/// <summary>
/// API service provides methods to retrieve/handle patients.
/// </summary>
public interface IPatientApiService
{
    /// <summary>
    /// Retrieves all existing patients.
    /// </summary>
    /// <returns>List of all existing patients.</returns>
    Task<IList<PatientDetailsBaseDto>> GetPatientsBasicInfoAsync();
}