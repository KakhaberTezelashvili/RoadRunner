using ProductionService.Shared.Dtos.Programs;

namespace ProductionService.Client.Services.Program;

/// <summary>
/// API service provides methods to retrieve/handle programs.
/// </summary>
public interface IProgramApiService
{
    /// <summary>
    /// Retrieves all the programs for the specified machine.
    /// </summary>
    /// <param name="machineKeyId">Machine key identifier.</param>
    /// <returns>The list of programs are returned.</returns>
    Task<IList<ProgramDetailsBaseDto>> GetProgramsByMachineAsync(int machineKeyId);
}