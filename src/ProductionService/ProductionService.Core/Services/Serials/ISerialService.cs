namespace ProductionService.Core.Services.Serials;

/// <summary>
/// Service provides methods to retrieve/handle serial numbers.
/// </summary>
public interface ISerialService
{
    /// <summary>
    /// Retrieves serial by key identifier.
    /// </summary>
    /// <param name="keyId">Primary key of the serial number.</param>
    /// <returns>Serial data.</returns>
    Task<SerialModel> GetByKeyIdAsync(int keyId);
}