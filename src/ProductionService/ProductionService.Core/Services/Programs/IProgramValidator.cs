using ProductionService.Shared.Dtos.Programs;

namespace ProductionService.Core.Services.Programs;

/// <summary>
/// Validator provides methods to validate programs.
/// </summary>
public interface IProgramValidator : IValidatorBase<ProgramModel>
{
    /// <summary>
    /// Validates program for machine asynchronous.
    /// </summary>
    /// <param name="programKeyId">Program key identifier.</param>
    /// <param name="machineKeyId">Machine key identifier.</param>
    /// <returns>Task&lt;ProgramDetails&gt;.</returns>
    Task<ProgramDetailsBaseDto> ProgramForMachineValidateAsync(int programKeyId, int machineKeyId);

    /// <summary>
    /// Validates programs by machine asynchronous.
    /// </summary>
    /// <param name="machineKeyId">Machine key identifier.</param>
    /// <returns>Task&lt;IList&lt;ProgramBasicDetails&gt;&gt;.</returns>
    Task<IList<ProgramDetailsBaseDto>> ProgramsByMachineValidateAsync(int machineKeyId);
}