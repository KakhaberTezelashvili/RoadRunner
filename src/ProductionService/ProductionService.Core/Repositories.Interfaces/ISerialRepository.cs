namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle serial numbers.
/// </summary>
public interface ISerialRepository : IRepositoryBase<SerialModel>
{
    /// <summary>
    /// Retrieves serial by key identifier.
    /// </summary>
    /// <param name="keyId">Primary key of the serial number.</param>
    /// <returns>Serial data/</returns>
    Task<SerialModel> GetByKeyIdAsync(int keyId);
}