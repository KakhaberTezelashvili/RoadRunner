using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Core.Services.Units.Patients;

/// <summary>
/// Service provides methods to retrieve/handle unit patient data.
/// </summary>
public interface IUnitPatientService
{
    /// <summary>
    /// Updates the unit with the specified patient.
    /// </summary>
    /// <param name="args">Unit patient arguments.</param>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <param name="userKeyId">User key identifier.</param>
    Task UpdatePatientAsync(int unitKeyId, int userKeyId, UnitPatientArgs args);
}