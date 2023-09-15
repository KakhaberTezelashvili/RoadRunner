using ProductionService.Shared.Dtos.Units;

namespace ProductionService.Client.Services.Units.Patients;

/// <summary>
/// API service provides methods to retrieve/handle unit patient data.
/// </summary>
public interface IUnitPatientApiService
{
    /// <summary>
    /// Updates the unit with the specified patient.
    /// </summary>
    /// <param name="args">Unit patient arguments.</param>
    /// <param name="unitKeyId">Unit key identifier.</param>
    Task UpdatePatientAsync(int unitKeyId, UnitPatientArgs args);
}