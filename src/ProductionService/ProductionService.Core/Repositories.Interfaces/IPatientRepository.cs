using ProductionService.Shared.Dtos.Patients;

namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle patients.
/// </summary>
public interface IPatientRepository : IRepositoryBase<PatientModel>
{
    /// <summary>
    /// Checks patient with specified identifier exist or not.
    /// </summary>
    /// <param name="patientKeyId">Patient key identifier.</param>
    /// <returns>
    /// returns true <see cref="bool"/> if the patient was found, otherwise returns false
    /// <c>null</c> otherwise.
    /// </returns>
    Task<bool> CheckPatientExistAsync(int patientKeyId);

    /// <summary>
    /// Retrieves the specified patient basic information.
    /// </summary>
    /// <param name="patientKeyId">Patient key identifier.</param>
    /// <returns>
    /// <see cref="PatientDetailsBaseDto"/> if the patient was found, <c>null</c> otherwise.
    /// </returns>
    Task<PatientDetailsBaseDto> GetBasicInfoByKeyIdAsync(int patientKeyId);

    /// <summary>
    /// Retrieves all existing patients.
    /// </summary>
    /// <returns>List of all existing patients.</returns>
    Task<IList<PatientDetailsBaseDto>> GetAllBasicInfosAsync();
}