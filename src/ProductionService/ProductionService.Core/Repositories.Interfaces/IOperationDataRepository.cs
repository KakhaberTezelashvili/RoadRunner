namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle operation data.
/// </summary>
public interface IOperationDataRepository
{
    /// <summary>
    /// Finds an operation from today with the specified patient and doctor (if doctorKeyId &gt;
    /// 0). If no such operation exists, creates a new dummy operation.
    /// </summary>
    /// <param name="patientKeyId">Patient key identifier.</param>
    /// <param name="factoryKeyId">
    /// Factory key identifier (needed for creating a dummy operation if such operation doesn't exist).
    /// </param>
    /// <param name="doctorKeyId">
    /// Doctor key identifier (can be set to 0/null if it shouldn't be taken into account).
    /// </param>
    /// <param name="userKeyId">
    /// User key identifier (needed for creating a dummy operation if such operation doesn't exist).
    /// </param>
    /// <param name="locationKeyId">
    /// Location key identifier (needed for creating a dummy operation if such operation doesn't exist).
    /// </param>
    /// <param name="positionLocationKeyId">
    /// Position/Location key identifier (needed for creating a dummy operation if such
    /// operation doesn't exist).
    /// </param>
    /// <returns>
    /// Key id of the existing operation if found or key id of the created dummy operation otherwise.
    /// </returns>
    Task<int> GetOperationKeyIdByPatientKeyIdAsync(int patientKeyId, int? factoryKeyId, int? doctorKeyId, int? userKeyId, int? locationKeyId, int? positionLocationKeyId);
}