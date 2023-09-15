using ProductionService.Shared.Dtos.Programs;

namespace ProductionService.Core.Repositories.Interfaces;

/// <summary>
/// Repository provides methods to retrieve/handle programs.
/// </summary>
public interface IProgramRepository : IRepositoryBase<ProgramModel>
{
    /// <summary>
    /// Retrieves program by key id.
    /// </summary>
    /// <param name="programKeyId">Program key id.</param>
    /// <returns>Program data.</returns>
    Task<ProgramModel> GetProgramAsync(int programKeyId);

    /// <summary>
    /// Retrieves all existing programs associated with the specified machine.
    /// </summary>
    /// <param name="machineKeyId">Machine key id.</param>
    /// <returns>List of programs.</returns>
    Task<IList<ProgramDetailsBaseDto>> GetProgramsByMachineAsync(int machineKeyId);

    /// <summary>
    /// Retrieves the information about the specified program.
    /// </summary>
    /// <param name="programKeyId">Program key id.</param>
    /// <param name="includeMachines">Defines if list of machines should be retrieved and included in details.</param>
    /// <returns>Program details if exists; <c>null</c> otherwise.</returns>
    Task<ProgramDetailsDto> GetProgramDetailsAsync(int programKeyId, bool includeMachines);
}