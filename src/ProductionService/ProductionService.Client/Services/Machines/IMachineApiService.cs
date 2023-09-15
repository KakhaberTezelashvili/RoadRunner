using ProductionService.Shared.Dtos.Machines;

namespace ProductionService.Client.Services.Machines;

/// <summary>
/// API service provides methods to retrieve/handle machines.
/// </summary>
public interface IMachineApiService
{
    /// <summary>
    /// Retrieves all the sterilizers for the specified location.
    /// </summary>
    /// <param name="locationKeyId">Location key identifier.</param>
    /// <param name="machineType">Machine type: Sterilizer, Washer.</param>
    /// <returns>The list of sterilizers are returned.</returns>
    Task<IList<MachineDetailsBaseDto>> GetMachinesByLocationAsync(int locationKeyId, MachineType machineType);
}