using ProductionService.Shared.Dtos.Programs;

namespace ProductionService.Core.Services.Programs;

/// <summary>
/// Service provides methods to retrieve/handle programs.
/// </summary>
public interface IProgramService
{
    /// <summary>
    /// Get program by key id.
    /// </summary>
    /// <param name="programKeyId">Program key id.</param>
    /// <returns>Program data.</returns>
    Task<ProgramModel> GetProgramAsync(int programKeyId);

    /// <summary>
    /// Retrieves all existing programs associated with the specified machine.
    /// </summary>
    /// <param name="machineKeyId">Machine key id.</param>
    /// <returns>List of programs.</returns>
    public Task<IList<ProgramDetailsBaseDto>> GetProgramsByMachineAsync(int machineKeyId);

    /// <summary>
    ///  Retrieves the program information for the specified machine.
    /// </summary>
    /// <param name="programKeyId">Program key id.</param>
    /// <param name="machineKeyId">Machine key id.</param>
    /// <returns>
    /// Result indicating the result of the operation;
    /// if the operation was successful, <see cref="ProgramDetailsBaseDto"/> returned in the response.
    /// <c>null</c> and an error specification otherwise.
    /// </returns>
    public Task<ProgramDetailsBaseDto> GetProgramForMachineAsync(int programKeyId, int machineKeyId);
}