namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle patient consumption.
/// </summary>
public interface IPatientConsumptionRepository : IRepositoryBase<PatientConsModel>
{
    /// <summary>
    /// Returns a tracked patient consumption asynchronous.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <returns>A task of patient consumption.</returns>
    Task<PatientConsModel> FindByUnitKeyIdAsync(int unitKeyId);

    /// <summary>
    /// Returns list of tracked patient consumption asynchronous.
    /// </summary>
    /// <param name="unitKeyId">Unit key identifier.</param>
    /// <returns>A task of list of patient consumption.</returns>
    Task<IEnumerable<PatientConsModel>> FindAllByUnitKeyIdAsync(int unitKeyId);
}